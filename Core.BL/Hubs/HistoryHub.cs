using Core.Common.Interfaces;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Core.BL.Hubs
{
    public class HistoryHub : Hub
    {
        private readonly ITransactionService _transactionService;

        public HistoryHub(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        public async Task SubscribeToTransactions(Guid accountId)
        {
            var accountIdStr = accountId.ToString();

            // Добавляем в группу по accountId
            await Groups.AddToGroupAsync(Context.ConnectionId, accountIdStr);

            // Отправляем текущую историю
            var transactions = await _transactionService.GetTransactions(accountId);
            await Clients.Caller.SendAsync("ReceiveTransactions", transactions);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            // Можно добавить логику очистки
            await base.OnDisconnectedAsync(exception);
        }
    }
}
