using System;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Testing.Shared
{
    public static class AssertEx
    {
        public static void EnsureExceptionMessageContains(Exception exception, params string[] expectedMessageParts)
        {
            var exceptionMessages = new StringBuilder();
            exceptionMessages.Append($"An Exception of type {exception.GetType()} was thrown, however the following message parts were not found in the exception message.");
            exceptionMessages.AppendLine($"The Actual Exception Message is: {exception.Message}");

            bool somePartNotFound = false;

            foreach (var part in expectedMessageParts)
            {
                if (!exception.Message.Contains(part))
                {
                    somePartNotFound = true;
                    exceptionMessages.AppendLine($"The Expected substring: {part}, was not found in the Exception Message.");
                }
            }

            if (somePartNotFound)
            {
                throw new AssertFailedException(exceptionMessages.ToString());
            }
        }

        public static void EnsureStringContains(string message, params string[] expectedMessageParts)
        {
            var exceptionMessages = new StringBuilder();
            exceptionMessages.AppendLine($"The Actual Message is: {message}");

            bool somePartNotFound = false;

            foreach (var part in expectedMessageParts)
            {
                if (!message.Contains(part))
                {
                    somePartNotFound = true;
                    exceptionMessages.AppendLine($"The Expected substring: {part}, was not found in the Message.");
                }
            }

            if (somePartNotFound)
            {
                throw new AssertFailedException(exceptionMessages.ToString());
            }
        }

        public static void EnsureExceptionMessageDoesNotContains(Exception exception, params string[] expectedMessageParts)
        {
            var exceptionMessages = new StringBuilder();
            exceptionMessages.Append($"An Exception of type {exception.GetType()} was thrown, however the message contains parts that were Not Expected");
            exceptionMessages.AppendLine($"The Actual Exception Message is: {exception.Message}");

            bool somePartFound = false;

            foreach (var part in expectedMessageParts)
            {
                if (exception.Message.Contains(part))
                {
                    somePartFound = true;
                    exceptionMessages.AppendLine($"The substring: {part}, was Not Expected to be contained in the Exception Message.");
                }
            }

            if (somePartFound)
            {
                throw new AssertFailedException(exceptionMessages.ToString());
            }
        }
    }
}
