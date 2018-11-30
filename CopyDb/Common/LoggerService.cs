using System;
using System.Diagnostics;
using NLog;

namespace CopyDb.Common
{
    public interface ILoggerService<T>
    {
        void Debug(string message);
        void Debug(Exception exception);
        void Debug(string message, Exception exception);

        void Error(string message);
        void Error(Exception exception);
        void Error(string message, Exception exception);

        void Warn(string message);
        void Warn(Exception exception);
        void Warn(string message, Exception exception);

        void Info(string message);
        void Info(Exception exception);
        void Info(string message, Exception exception);

        void Trace(string message);
        void Trace(Exception exception);
        void Trace(string message, Exception exception);
    }

    public class LoggerService<T> : ILoggerService<T>
    {
        internal static ILoggerService<T> Logger()
        {
            return new LoggerService<T>();
        }

        private readonly ILogger _logger = LogManager.GetLogger(typeof(T).FullName);

        public void Debug(Exception exception)
        {
            _logger.Debug(exception.Demystify());
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Debug(string message, Exception exception)
        {
            _logger.Debug(message);
            _logger.Debug(exception.Demystify());
        }

        public void Error(Exception exception)
        {
            _logger.Error(exception.Demystify());
        }

        public void Error(string message)
        {
            _logger.Error(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(message);
            _logger.Error(exception.Demystify());
        }

        public void Info(Exception exception)
        {
            _logger.Info(exception.Demystify());
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Info(string message, Exception exception)
        {
            _logger.Info(message);
            _logger.Info(exception.Demystify());
        }

        public void Trace(Exception exception)
        {
            _logger.Trace(exception.Demystify());
        }

        public void Trace(string message)
        {
            _logger.Trace(message);
        }

        public void Trace(string message, Exception exception)
        {
            _logger.Trace(message);
            _logger.Trace(exception.Demystify());
        }

        public void Warn(Exception exception)
        {
            _logger.Warn(exception.Demystify());
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Warn(string message, Exception exception)
        {
            _logger.Warn(message);
            _logger.Warn(exception.Demystify());
        }
    }
}
