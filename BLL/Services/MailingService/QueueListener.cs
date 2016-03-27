﻿using System;
using System.Threading;
using System.Threading.Tasks;
using BLL.Services.MailingService.Interfaces;
using BLL.Services.MailingService.Types;

namespace BLL.Services.MailingService
{
    /// <summary>
    /// Adds and removes mails to queue
    /// </summary>
    public static class QueueListener
    {
        #region private
        static IMailingService m_sender;

        static ILogger m_logger;

        static Int32 m_maxSendTries;

        static Task m_backgrndWorker;

        static CancellationTokenSource m_backgrndWorkerCancellationTokenSource;

        static CancellationToken m_backgrndWorkerCancellationToken;
        #endregion


        public static void Start(IMailingService mailingService, ILogger logger)
        {
            m_sender = mailingService;
            m_sender.IgnoreQueue(); //doesn't add message to queue
            m_logger = logger;
            ////m_backgrndWorker = new Thread(() => BackgrndAction()); //setting thread action
            ////m_backgrndWorker.IsBackground = true; //this thread will work in background
            m_backgrndWorkerCancellationTokenSource = new CancellationTokenSource();
            m_backgrndWorkerCancellationToken = m_backgrndWorkerCancellationTokenSource.Token;
            m_backgrndWorker = new Task(BackgrndAction, m_backgrndWorkerCancellationToken);
            m_maxSendTries = 3;
            StartBackgrndProcessing(); //starts listener
        }

        public static void Stop()
        {
            ////m_backgrndWorker.Abort();
            m_backgrndWorkerCancellationTokenSource.Cancel();
            MailQueue.Stop();
            m_logger?.Log($"[{DateTime.Now.ToLongTimeString()}] Listener stopped.");
            m_logger?.Dispose();
        }

        private static void StartBackgrndProcessing()
        {
            m_backgrndWorker.Start();
            m_logger?.Log($"[{DateTime.Now.ToLongTimeString()}] Listener started.");
        }

        /// <summary>
        /// Action that executes in thread
        /// </summary>
        private static void BackgrndAction()
        {
            ////try
            {
                QueuedMessage message = null; //message to resend
                SendStatus sendResult;
                while (true)
                {
                    if (IsCancelledBackgrndAction())
                        break;

                    message = MailQueue.Queue.GetMessage(); //getting message from queue

                    if (message != null)
                    {
                        if (message.TimeToRemove > DateTime.Now && message.SendingAttempts < m_maxSendTries) //checks if it can send message
                        {
                            if (IsCancelledBackgrndAction())
                                break;

                            sendResult = m_sender.SendMail(message.Message);

                            if (sendResult.Status == MessageStatus.Error && ++message.SendingAttempts <= m_maxSendTries) //another sending fail
                                MailQueue.Queue.AddMessage(message);
                            else if (sendResult.Status == MessageStatus.Sent)
                                m_logger?.Log($"[{DateTime.Now.ToLongTimeString()}] Successfully sent a message to {message.GetAllRecievers()}.");
                        }
                        else
                            m_logger?.Log($"[{DateTime.Now.ToLongTimeString()}] Sending of message to {message.GetAllRecievers()} failed.");
                    }
                }
            }
            ////catch (ThreadAbortException)
            ////{
            ////    m_logger?.Log($"[{DateTime.Now.ToLongTimeString()}] Thread stopped.");
            ////}
        }

        private static bool IsCancelledBackgrndAction()
        {
            if (!m_backgrndWorkerCancellationToken.IsCancellationRequested)
                return false;

            m_logger?.Log($"[{DateTime.Now.ToLongTimeString()}] Thread stopped.");
            return true;
        }
    }
}
