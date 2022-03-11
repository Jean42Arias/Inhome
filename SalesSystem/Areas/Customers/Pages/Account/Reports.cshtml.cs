using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SalesSystem.Areas.Customers.Models;
using SalesSystem.Data;
using SalesSystem.Library;

namespace SalesSystem.Areas.Customers.Pages.Account
{
    [Authorize]
    public class ReportsModel : PageModel
    {
        private LCustomers _customer;
        private static int idClient = 0;
        public static string Money = "$";
        private static string _errorMessage;
        public static InputModelRegister _dataClient;
        private LCodes _codes;
        private ApplicationDbContext _context;
        private UserManager<IdentityUser> _userManager;

        public ReportsModel(
            UserManager<IdentityUser> userManager,
            ApplicationDbContext context)
        {
            _context = context;
            _userManager = userManager;
            _codes = new LCodes();
            _customer = new LCustomers(context);
        }
        public IActionResult OnGet(int id)
        {
            if (idClient == 0)
            {
                idClient = id;
            }
            else
            {
                if (idClient != id)
                {
                    idClient = 0;
                    return Redirect("/Customers/Customers?area=Customers");
                }
            }
            _dataClient = _customer.getTClientReport(id);
            Input = new InputModel
            {
                DataClient = _dataClient,
                ErrorMessage = _errorMessage
            };
            _errorMessage = "";
            return Page();
        }
        [BindProperty]
        public InputModel Input { get; set; }
        public class InputModel
        {
            public string Money { get; set; } = "$";
            [Required(ErrorMessage = "Seleccione una opcion.")]
            public int RadioOptions { get; set; }

            [Required(ErrorMessage = "El campo pago es obligatorio.")]
            [RegularExpression(@"^[0-9]+([.][0-9]+)?$", ErrorMessage = "El pago no es correcto.")]
            public Decimal Payment { set; get; }

            public InputModelRegister DataClient { get; set; }
            [TempData]
            public string ErrorMessage { get; set; }
        }
        public async Task<IActionResult> OnPostAsync()
        {
            var idUser = _userManager.GetUserId(User);
            switch (Input.RadioOptions)
            {
                case 1:
                    if (_dataClient.Debt.Equals(0.0m))
                    {
                        _errorMessage = "El cliente no contiene deuda";
                    }
                    else
                    {
                        String _change = "";
                        Decimal _currentDebt = 0.0m, change;
                        if (Input.Payment >= _dataClient.Monthly)
                        {
                            var _ticket = _codes.codesTickets(_dataClient.Ticket);
                            var _nameCliente = $"{_dataClient.Name} {_dataClient.LastName}";
                            var dateNow = DateTime.Now;
                            var nowDate = $"{dateNow.Day}/{dateNow.Month}/{dateNow.Year}";
                            var user = _context.TUsers.Where(u => u.IdUser.Equals(idUser)).ToList();
                            var name = $"{user[0].Name} {user[0].LastName}";
                            if (Input.Payment.Equals(_dataClient.CurrentDebt) || Input.Payment > _dataClient.CurrentDebt)
                            {
                                change = Input.Payment - _dataClient.CurrentDebt;
                                _change = String.Format("{0:#,###,###,##0.00####}", change);
                                _errorMessage = $"Cambio para el cliente {Money}{_change}";
                                _currentDebt = 0.0m;
                            }
                            else
                            {
                                change = Input.Payment - _dataClient.Monthly;
                                _change = String.Format("{0:#,###,###,##0.00####}", change);
                                _errorMessage = $"Cambio para el cliente {Money}{_change}";
                                _currentDebt = _dataClient.CurrentDebt - _dataClient.Monthly;
                            }
                            var strategy = _context.Database.CreateExecutionStrategy();
                            await strategy.ExecuteAsync(async () => {
                                using (var transaction = _context.Database.BeginTransaction())
                                {
                                    try
                                    {
                                        var _payment = String.Format("{0:#,###,###,##0.00####}", Input.Payment);
                                        var _debt = String.Format("{0:#,###,###,##0.00####}", _dataClient.Debt);
                                        var currentDebt = String.Format("{0:#,###,###,##0.00####}", _currentDebt);
                                        //var cambio = String.Format("{0:#,###,###,##0.00####}", _change);
                                        var _currentDebtClient = String.Format("{0:#,###,###,##0.00####}", _dataClient.CurrentDebt);

                                        var _monthly = String.Format("{0:#,###,###,##0.00####}", _dataClient.Monthly);
                                        var date = DateTime.Now.AddMonths(1);

                                        var _deadline = _dataClient.CurrentDebt.Equals(0.0m) ? "--/--/--" : $"{date.Day}/{date.Month}/{ date.Year}";
                                        var client = _context.TClients.Where(c => c.IdClient.Equals(_dataClient.IdClient)).ToList().ElementAt(0);
                                        var report = new TReports_clients
                                        {
                                            IdReport = _dataClient.IdReport,
                                            Debt = _dataClient.Debt,
                                            DateDebt = _dataClient.DateDebt,
                                            Monthly = _dataClient.Monthly,
                                            Change = change,
                                            LastPayment = Input.Payment,
                                            DatePayment = dateNow,
                                            CurrentDebt = _currentDebt,
                                            Ticket = _ticket,
                                            Deadline = date,
                                            TClients = client
                                        };
                                        _context.Update(report);
                                        _context.SaveChanges();

                                        var payments = new TPayments_clients
                                        {
                                            Debt = _dataClient.Debt,
                                            Change = change,
                                            Payment = Input.Payment,
                                            Date = dateNow,
                                            CurrentDebt = _currentDebt,
                                            Ticket = _ticket,
                                            Deadline = date,
                                            IdUser = idUser,
                                            User = name,
                                            IdClient = _dataClient.IdClient
                                        };
                                        _context.Add(payments);
                                        _context.SaveChanges();

                                        LTicket Ticket1 = new LTicket();
                                        Ticket1.AbreCajon();  //abre el cajon
                                        Ticket1.TextoCentro("Sistema de ventas PDHN"); // imprime en el centro 
                                        Ticket1.TextoIzquierda("Direccion");
                                        Ticket1.TextoIzquierda("La Ceiba, Atlantidad");
                                        Ticket1.TextoIzquierda("Tel 658912146");
                                        Ticket1.LineasGuion();
                                        Ticket1.TextoCentro("FACTURA"); // imprime en el centro 
                                        Ticket1.LineasGuion();
                                        Ticket1.TextoIzquierda($"Factura:{ _ticket}");
                                        Ticket1.TextoIzquierda($"Cliente:{ _nameCliente}");
                                        Ticket1.TextoIzquierda($"Fecha:{nowDate}");
                                        Ticket1.TextoIzquierda($"Usuario:{name}");
                                        Ticket1.LineasGuion();
                                        Ticket1.TextoCentro($"Su cretito {Money}{_debt}");
                                        Ticket1.TextoExtremo("Cuotas por mese:", $"{Money}{_monthly}");
                                        Ticket1.TextoExtremo("Deuda anterior:", $"{Money}{_currentDebtClient}");
                                        Ticket1.TextoExtremo("Pago:", $"{Money}{_payment}");
                                        Ticket1.TextoExtremo("Cambio:", $"{Money}{_change}");
                                        Ticket1.TextoExtremo("Deuda actual:", $"{Money}{currentDebt}");
                                        Ticket1.TextoExtremo("Proximo pago:", $"{_deadline}");
                                        Ticket1.TextoCentro("PDHN");
                                        Ticket1.CortaTicket(); // corta el ticket

                                        Ticket1.ImprimirTicket("Microsoft XPS Document Writer");

                                        transaction.Commit();
                                    }
                                    catch (Exception ex)
                                    {
                                        _errorMessage = ex.Message;
                                        transaction.Rollback();
                                    }
                                }
                            });

                        }
                        else
                        {
                            var monthly = String.Format("{0:#,###,###,##0.00####}", _dataClient.Monthly);
                            _errorMessage = $"El pago debe ser {Money}{monthly}";
                        }
                    }
                    break;
            }
            return Redirect("/Customers/Reports?id=" + idClient);
        }
    }
}
