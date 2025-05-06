using Core.Common.Dtos;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Core.BL.Services
{
    public class FirebaseService
    {
        public FirebaseService()
        {
            if (FirebaseApp.DefaultInstance == null)
            {
                FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile("../Common/Firebase/patterns2025-f0f45-firebase-adminsdk-fbsvc-13dbd81da1.json")
                    
                });
            }
        }

        public async Task SendPushNotification(List<string> deviceTokens, Transaction transaction)
        {
            var message = new MulticastMessage()
            {
                Notification = new Notification
                {
                    Title = "Перевод получен",
                    Body = $"Перевод {transaction.Amount}{transaction.Currency} от {transaction.RelatedAccountId} "
                },
                Data = new Dictionary<string, string>
                {
                    ["amount"] = transaction.Amount.ToString(),
                    ["currency"] = transaction.Currency.ToString(),
                    ["sender"] = transaction.RelatedAccountId.ToString(),
                    ["date"] = transaction.CreatedAt.ToString()
                },
                Tokens = deviceTokens
            };

            await FirebaseMessaging.DefaultInstance.SendEachForMulticastAsync(message);
        }
    }
}
