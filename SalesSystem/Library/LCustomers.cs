using SalesSystem.Areas.Customers.Models;
using SalesSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SalesSystem.Library
{
    public class LCustomers : ListObject
    {
        public LCustomers(ApplicationDbContext context)
        {
            _context = context;
        }
        public List<InputModelRegister> getTClients(String valor, int id)
        {
            List<TClients> listTClients;
            var clientsList = new List<InputModelRegister>();
            if (valor == null && id.Equals(0))
            {
                listTClients = _context.TClients.ToList();
            }
            else
            {
                if (id.Equals(0))
                {
                    listTClients = _context.TClients.Where(u => u.Nid.StartsWith(valor) || u.Name.StartsWith(valor) ||
              u.LastName.StartsWith(valor) || u.Email.StartsWith(valor)).ToList();
                }
                else
                {
                    listTClients = _context.TClients.Where(u => u.IdClient.Equals(id)).ToList();
                }
            }
            if (!listTClients.Count.Equals(0))
            {
                foreach (var item in listTClients)
                {
                    clientsList.Add(new InputModelRegister
                    {
                        IdClient = item.IdClient,
                        Nid = item.Nid,
                        Name = item.Name,
                        LastName = item.LastName,
                        Email = item.Email,
                        Phone = item.Phone,
                        Credit = item.Credit,
                        Direction = item.Direction,
                        Image = item.Image,
                    });
                }
            }
            return clientsList;
        }
        public List<TClients> getTClient(String Nid)
        {
            var listTClients = new List<TClients>();
            using (var dbContext = new ApplicationDbContext())
            {
                listTClients = dbContext.TClients.Where(u => u.Nid.Equals(Nid)).ToList();
            }
                
            return listTClients;
        }
        public InputModelRegister getTClientReport(int id)
        {
            var dataClients = new InputModelRegister();
            using (var dbContext = new ApplicationDbContext())
            {
                var query = dbContext.TClients.Join(dbContext.TReports_clients,
               c => c.IdClient, r => r.TClientsIdClient, (c, r) => new
               {
                   c.IdClient,
                   c.Nid,
                   c.Name,
                   c.LastName,
                   c.Phone,
                   c.Email,
                   c.Direction,
                   c.Credit,
                   r.IdReport,
                   r.Debt,
                   r.Monthly,
                   r.Change,
                   r.CurrentDebt,
                   r.DatePayment,
                   r.LastPayment,
                   r.Ticket,
                   r.Deadline,
               }).Where(c => c.IdClient.Equals(id)).ToList();
                if (!query.Count.Equals(0))
                {
                    var data = query.ToList().Last();
                    dataClients = new InputModelRegister
                    {
                        IdClient = data.IdClient,
                        Nid = data.Nid,
                        Name = data.Name,
                        LastName = data.LastName,
                        Phone = data.Phone,
                        Email = data.Email,
                        Direction = data.Direction,
                        Credit = data.Credit,
                        IdReport = data.IdReport,
                        Debt = data.Debt,
                        Monthly = data.Monthly,
                        Change = data.Change,
                        CurrentDebt = data.CurrentDebt,
                        DatePayment = data.DatePayment,
                        LastPayment = data.LastPayment,
                        Ticket = data.Ticket,
                        Deadline = data.Deadline,
                    };
                }
            }
            return dataClients;
        }
    }
}
