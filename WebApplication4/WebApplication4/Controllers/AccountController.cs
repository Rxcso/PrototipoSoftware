﻿using System;
using System.Globalization;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using WebApplication4.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Net.NetworkInformation;
using System.Web.Security;

namespace WebApplication4.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private inf245netsoft db = new inf245netsoft();


        public AccountController()
        {
        }

        public AccountController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                TempData["tipo"] = "alert alert-warning";
                TempData["message"] = "Colocar correo y contraseña con el formato indicado";
                return Redirect("~/Home/Index");
            }

            // This doesn't count login failures towards account lockout
            // To enable password failures to trigger account lockout, change to shouldLockout: true
            CuentaUsuario cuentausuario = db.CuentaUsuario.Find(model.Email);
            PuntoVenta punt = new PuntoVenta();
            try
            {
                if (cuentausuario.codPerfil == 2)
                {
                    string macAddresses = "";
                    DateTime hora = DateTime.Now;
                    List<Turno> turnos = db.Turno.Where(c => c.usuario == cuentausuario.correo).ToList();
                    turnos = turnos.Where(c => c.fecha.Date == DateTime.Today.Date).ToList();
                    foreach (Turno turno in turnos)
                    {
                        TurnoSistema tS = turno.TurnoSistema;
                        string horaInicio = tS.horIni;
                        string horaFin = tS.horFin;
                        DateTime horaInicioDate = DateTime.ParseExact(horaInicio, "H:m:s", null);
                        DateTime horaFinDate = DateTime.ParseExact(horaFin, "H:m:s", null);
                        if (horaInicioDate <= hora && hora <= horaFinDate)
                        {
                            macAddresses = turno.PuntoVenta.dirMAC;
                            break;
                        }
                    }

                    /*foreach (NetworkInterface nic in NetworkInterface.GetAllNetworkInterfaces())
                    {
                        if (nic.OperationalStatus == OperationalStatus.Up)
                        {
                            macAddresses += nic.GetPhysicalAddress().ToString();
                            break;
                        }
                    }*/
                    if (macAddresses != "")
                    {
                        List<PuntoVenta> lpu = db.PuntoVenta.Where(c => c.dirMAC == macAddresses).ToList();
                        punt = lpu.First();
                    }
                }

            }
            catch (Exception ex)
            {
                try
                {
                    if (cuentausuario.codPerfil == 2)
                    {
                        TempData["tipo"] = "alert alert-warning";
                        TempData["message"] = "Iniciar Sesión desde un punto de venta registrado.";
                    }
                }
                catch (Exception ex2)
                {
                    TempData["tipo"] = "alert alert-warning";
                    TempData["message"] = "Correo no registrado.";
                }

                return Redirect("~/Home/Index");
            }

            if (cuentausuario.codPerfil != 1 && cuentausuario.estado == false) return Redirect("~/Home/Index");

            var result = await SignInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, shouldLockout: false);
            Session["orgPago"] = null;
            Session["orgPago2"] = null;
            Session["vendAsig"] = null;
            Session["Pagos2"] = null;
            Session["ListaTurnoVendedor"] = null;
            Session["Pagos"] = null;
            Session["Pendiente"] = null;
            Session["Pendiente2"] = null;
            Session["EventoSeleccionadoPago"] = null;
            Session["EventoSeleccionadoPago2"] = null;
            TempData["tipo"] = "alert alert-warning";
            TempData["message"] = "Logueo Incorrecto";

            switch (result)
            {
                case SignInStatus.Success:
                    TempData["tipo"] = "alert alert-success";
                    TempData["message"] = "Logueado Correctamente";
                    if (cuentausuario.codPerfil == 1)
                    {
                        Session["UsuarioLogueado"] = cuentausuario;
                        return Redirect("~/Home/Index");
                    }
                    else
                    {
                        if (cuentausuario.codPerfil == 2)
                        {
                            Turno tu = null;
                            DateTime hoy = DateTime.Now;
                            int idPunto = 1;
                            TimeSpan da = DateTime.Now.TimeOfDay;
                            TurnoSistema ts = new TurnoSistema();
                            List<TurnoSistema> listaturno = db.TurnoSistema.AsNoTracking().Where(c => c.activo == true).ToList();
                            for (int i = 0; i < listaturno.Count; i++)
                            {
                                if ((TimeSpan.Parse(listaturno[i].horIni) < da) && (TimeSpan.Parse(listaturno[i].horFin) > da))
                                {
                                    ts = listaturno[i];
                                }
                            }
                            Session["ListaTurnoHoy"] = null;
                            List<Turno> liT = new List<Turno>();
                            List<Turno> liTV = new List<Turno>();
                            liT = db.Turno.AsNoTracking().Where(j => j.usuario == cuentausuario.usuario && j.codPuntoVenta == punt.codPuntoVenta && j.codTurnoSis == ts.codTurnoSis).ToList();
                            liTV = db.Turno.AsNoTracking().Where(j => j.codPuntoVenta == punt.codPuntoVenta && j.codTurnoSis == ts.codTurnoSis).ToList();
                            liTV = liTV.Where(c => c.fecha == hoy.Date).ToList();
                            Session["ListaTurnoHoy"] = liTV;
                            Session["PuntoVentaLoguedo"] = punt.codPuntoVenta;
                            if (liT != null)
                            {
                                for (int i = 0; i < liT.Count; i++)
                                {
                                    if (liT[i].fecha.Date == hoy.Date)
                                    {
                                        tu = liT[i];
                                        Session["TurnoHoy"] = tu;
                                        break;
                                    }
                                }
                            }
                            if (tu != null && tu.estado == "Pendiente")
                            {
                                int idPol = 4;
                                int limite = (int)db.Politicas.Find(idPol).valor;
                                TimeSpan time1 = TimeSpan.FromMinutes(limite);
                                TimeSpan horalimite = TimeSpan.Parse(ts.horIni);
                                TimeSpan hora1 = horalimite.Add(time1);
                                //db.Entry(tu).State = EntityState.Modified;
                                if (hora1 > da)
                                {
                                    tu.estado = "Asistio";
                                }
                                else
                                {
                                    tu.estado = "Tarde";
                                }
                                tu.horaRegistro = da.ToString(@"hh\:mm\:ss");
                                Session["MensajeIngresoTurno"] = "Hora de ingreso registrada a las: " + da.ToString(@"hh\:mm\:ss") + " hs";
                                db.SaveChanges();
                                //db.Entry(tu).State = EntityState.Detached;
                            }
                        }
                        Session["UsuarioLogueado"] = cuentausuario;
                        return Redirect("~/Home/Index2");
                    }
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                case SignInStatus.Failure:
                    return Redirect("~/Home/Index");
                default:
                    ModelState.AddModelError("", "Invalid login attempt.");
                    return Redirect("~/Home/Index");
            }
        }
        //
        // GET: /Account/VerifyCode
        [AllowAnonymous]
        public async Task<ActionResult> VerifyCode(string provider, string returnUrl, bool rememberMe)
        {
            // Require that the user has already logged in via username/password or external login
            if (!await SignInManager.HasBeenVerifiedAsync())
            {
                return View("Error");
            }
            return View(new VerifyCodeViewModel { Provider = provider, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/VerifyCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // The following code protects for brute force attacks against the two factor codes. 
            // If a user enters incorrect codes for a specified amount of time then the user account 
            // will be locked out for a specified amount of time. 
            // You can configure the account lockout settings in IdentityConfig
            var result = await SignInManager.TwoFactorSignInAsync(model.Provider, model.Code, isPersistent: model.RememberMe, rememberBrowser: model.RememberBrowser);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(model.ReturnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.Failure:
                default:
                    ModelState.AddModelError("", "Invalid code.");
                    return View(model);
            }
        }

        //
        // GET: /Account/Register
        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public ActionResult RegisterClient()
        {
            //destacados
            List<Eventos> listaDestacados = new List<Eventos>(0);
            try
            {
                listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null && c.estado != null && c.estado.CompareTo("Activo") == 0)).ToList();
            }
            catch (Exception ex)
            {

            }
            ViewBag.ListaDestacados = listaDestacados;
            return View("RegisterClient");
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterClient(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                int errorr = 0;
                if (model.tipoDoc == 1)
                {
                    if (model.codDoc.Length != 8)
                    {
                        ModelState.AddModelError("codDoc", "El DNI debe tener 8 dígitos");
                        errorr = 1;
                    }
                    List<CuentaUsuario> lcu = db.CuentaUsuario.Where(c => c.tipoDoc == model.tipoDoc && c.codDoc == model.codDoc).ToList();
                    if (lcu.Count > 0)
                    {
                        ModelState.AddModelError("codDoc", "DNI ya utilizado");
                        errorr = 1;
                    }
                    if (model.fechaNac > DateTime.Today || model.fechaNac < Convert.ToDateTime("01/01/1900"))
                    {
                        ModelState.AddModelError("fechaNac", "La fecha con rango inválido");
                        errorr = 1;
                    }
                }
                else
                {
                    if (model.codDoc.Length != 12)
                    {
                        ModelState.AddModelError("codDoc", "El Pasaporte debe tener 12 dígitos");
                        errorr = 1;
                    }
                    List<CuentaUsuario> lcu = db.CuentaUsuario.Where(c => c.tipoDoc == model.tipoDoc && c.codDoc == model.codDoc).ToList();
                    if (lcu.Count > 0)
                    {
                        ModelState.AddModelError("codDoc", "Pasaporte ya utilizado");
                        errorr = 1;
                    }
                    if (model.fechaNac > DateTime.Today || model.fechaNac < Convert.ToDateTime("01/01/1900"))
                    {
                        ModelState.AddModelError("fechaNac", "La fecha con rango inválido");
                        errorr = 1;
                    }
                }

                if (errorr != 1)
                {
                    var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                    var result = await UserManager.CreateAsync(user, model.Password);
                    if (result.Succeeded)
                    {
                        var currentUser = UserManager.FindByName(user.UserName);
                        UserManager.AddToRole(user.Id, "Cliente");
                        CuentaUsuario cuentausuario = new CuentaUsuario();

                        cuentausuario.correo = model.Email;
                        cuentausuario.apellido = model.apellido;
                        cuentausuario.codDoc = model.codDoc;
                        cuentausuario.codPerfil = 1;
                        cuentausuario.contrasena = model.Password;
                        cuentausuario.direccion = model.direccion;
                        cuentausuario.estado = true;
                        cuentausuario.fechaNac = model.fechaNac;
                        cuentausuario.nombre = model.nombre;
                        cuentausuario.puntos = 0;
                        cuentausuario.sexo = model.sexo;
                        cuentausuario.telefono = model.telefono;
                        cuentausuario.telMovil = model.telMovil;
                        cuentausuario.tipoDoc = model.tipoDoc;
                        cuentausuario.usuario = model.Email;

                        db.CuentaUsuario.Add(cuentausuario);
                        db.SaveChanges();
                        Session["UsuarioLogueado"] = cuentausuario;
                        EmailController.EnviarCorreoRegistro(model.Email);
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                        // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                        // Send an email with this link
                        // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                        // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                        // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                        TempData["tipo"] = "alert alert-success";
                        TempData["message"] = "Registro Exitoso!";
                        return RedirectToAction("Index", "Home");
                        //return View("~/Views/Home/Index.cshtml");
                    }

                    foreach (var error in result.Errors)
                    {
                        if (!error.Contains("nombre"))
                            ModelState.AddModelError("", error);
                    }
                }
                else
                    return View(model);

            }
            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterVendedor(RegisterViewModel model)
        {
            CuentaUsuario cue = null;
            cue = db.CuentaUsuario.Find(model.Email);
            if (cue != null)
            {
                TempData["MessageErrorVendedor"] = "Ya existe una cuenta con ese correo";
                return RedirectToAction("Index", "Empleado");
            }
            List<CuentaUsuario> lcu = db.CuentaUsuario.Where(c => c.tipoDoc == model.tipoDoc && c.codDoc == model.codDoc).ToList();
            if (lcu == null || lcu.Count > 0)
            {
                TempData["MessageErrorVendedor"] = "Ya existe un cuenta registrada con ese DNI";
                return RedirectToAction("Index", "Empleado");
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var currentUser = UserManager.FindByName(user.UserName);
                    UserManager.AddToRole(user.Id, "Vendedor");
                    CuentaUsuario cuentausuario = new CuentaUsuario();

                    cuentausuario.correo = model.Email;
                    cuentausuario.apellido = model.apellido;
                    cuentausuario.codDoc = model.codDoc;
                    cuentausuario.codPerfil = 2;
                    //cuentausuario.contrasena = user.PasswordHash;
                    cuentausuario.direccion = model.direccion;
                    cuentausuario.estado = true;
                    cuentausuario.fechaNac = model.fechaNac;
                    cuentausuario.nombre = model.nombre;
                    cuentausuario.puntos = 0;
                    cuentausuario.sexo = model.sexo;
                    cuentausuario.telefono = model.telefono;
                    cuentausuario.telMovil = model.telMovil;
                    cuentausuario.tipoDoc = model.tipoDoc;
                    cuentausuario.usuario = model.Email;
                    db.CuentaUsuario.Add(cuentausuario);
                    db.SaveChanges();

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return RedirectToAction("Index", "Empleado");
                    //return View("~/Views/Home/Index.cshtml");
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Empleado");
        }

        [HttpPost]
        [Authorize(Roles = "Administrador")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> RegisterPromotor(RegisterViewModel model)
        {
            CuentaUsuario cue = null;
            cue = db.CuentaUsuario.Find(model.Email);
            if (cue != null)
            {
                TempData["MessageErrorPromotor"] = "Ya existe una cuenta con ese correo";
                return RedirectToAction("Index", "Empleado");
            }
            List<CuentaUsuario> lcu = db.CuentaUsuario.Where(c => c.tipoDoc == model.tipoDoc && c.codDoc == model.codDoc).ToList();
            if (lcu == null || lcu.Count > 0)
            {
                TempData["MessageErrorPromotor"] = "Ya existe un cuenta registrada con ese DNI";
                return RedirectToAction("Index", "Empleado");
            }
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    var currentUser = UserManager.FindByName(user.UserName);
                    UserManager.AddToRole(user.Id, "Promotor");
                    CuentaUsuario cuentausuario = new CuentaUsuario();

                    cuentausuario.correo = model.Email;
                    cuentausuario.apellido = model.apellido;
                    cuentausuario.codDoc = model.codDoc;
                    cuentausuario.codPerfil = 3;
                    //cuentausuario.contrasena = user.PasswordHash;
                    cuentausuario.direccion = model.direccion;
                    cuentausuario.estado = true;
                    cuentausuario.fechaNac = model.fechaNac;
                    cuentausuario.nombre = model.nombre;
                    cuentausuario.puntos = 0;
                    cuentausuario.sexo = model.sexo;
                    cuentausuario.telefono = model.telefono;
                    cuentausuario.telMovil = model.telMovil;
                    cuentausuario.tipoDoc = model.tipoDoc;
                    cuentausuario.usuario = model.Email;
                    db.CuentaUsuario.Add(cuentausuario);
                    db.SaveChanges();
                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");
                    return RedirectToAction("Index", "Empleado");
                    //return View("~/Views/Home/Index.cshtml");
                }
                AddErrors(result);
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Empleado");
        }

        //
        // GET: /Account/ConfirmEmail
        [AllowAnonymous]
        public async Task<ActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return View("Error");
            }
            var result = await UserManager.ConfirmEmailAsync(userId, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }

        //
        // GET: /Account/ForgotPassword
        [AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            return View();
        }

        //
        // POST: /Account/ForgotPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return View("ForgotPasswordConfirmation");
                }
                // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                // Send an email with this link
                // string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                // var callbackUrl = Url.Action("ResetPassword", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);		
                // await UserManager.SendEmailAsync(user.Id, "Reset Password", "Please reset your password by clicking <a href=\"" + callbackUrl + "\">here</a>");
                // return RedirectToAction("ForgotPasswordConfirmation", "Account");
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        // GET: /Account/ForgotPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        //
        // GET: /Account/ResetPassword
        [AllowAnonymous]
        public ActionResult ResetPassword(string code)
        {
            return code == null ? View("Error") : View();
        }

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation", "Account");
            }
            AddErrors(result);
            return View();
        }

        //
        // GET: /Account/ResetPasswordConfirmation
        [AllowAnonymous]
        public ActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        //
        // POST: /Account/ExternalLogin
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        //
        // GET: /Account/SendCode
        [AllowAnonymous]
        public async Task<ActionResult> SendCode(string returnUrl, bool rememberMe)
        {
            var userId = await SignInManager.GetVerifiedUserIdAsync();
            if (userId == null)
            {
                return View("Error");
            }
            var userFactors = await UserManager.GetValidTwoFactorProvidersAsync(userId);
            var factorOptions = userFactors.Select(purpose => new SelectListItem { Text = purpose, Value = purpose }).ToList();
            return View(new SendCodeViewModel { Providers = factorOptions, ReturnUrl = returnUrl, RememberMe = rememberMe });
        }

        //
        // POST: /Account/SendCode
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> SendCode(SendCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            // Generate the token and send it
            if (!await SignInManager.SendTwoFactorCodeAsync(model.SelectedProvider))
            {
                return View("Error");
            }
            return RedirectToAction("VerifyCode", new { Provider = model.SelectedProvider, ReturnUrl = model.ReturnUrl, RememberMe = model.RememberMe });
        }

        //
        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
                    return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { Email = loginInfo.Email });
            }
        }

        //
        // POST: /Account/ExternalLoginConfirmation
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await AuthenticationManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await UserManager.AddLoginAsync(user.Id, info.Login);
                    if (result.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        //
        // POST: /Account/LogOff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            Session["UsuarioLogueado"] = null;
            Session["TurnoHoy"] = null;
            Session["MensajeIngresoTurno"] = null;
            Session["CarritoCreado"] = false;
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/ExternalLoginFailure
        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_userManager != null)
                {
                    _userManager.Dispose();
                    _userManager = null;
                }

                if (_signInManager != null)
                {
                    _signInManager.Dispose();
                    _signInManager = null;
                }
            }

            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Destacados()
        {
            //para que se carguen los destacados al lado
            List<Eventos> listaDestacados = new List<Eventos>(0);
            try
            {
                listaDestacados = db.Eventos.AsNoTracking().Where(c => (c.ImagenDestacado != null && c.estado != null && c.estado.CompareTo("Activo") == 0)).ToList();
            }
            catch (Exception ex)
            {
            }
            ViewBag.ListaDestacados = listaDestacados;
            return View();
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {

                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion
    }
}