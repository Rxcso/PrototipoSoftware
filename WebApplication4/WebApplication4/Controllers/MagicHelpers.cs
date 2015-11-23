using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApplication4.Controllers
{
    public class MagicHelpers
    {
        //Padre de todas las categorias
        public const int Categorias = 1;
        //Constante para numero de decimales de tipo de cambio
        public const int ConstanteTipoCambio = 10000;
        //Identificador de un evento nuevo
        public const string NuevoEventoImagen = "/Doesnt/Exists/Yet";
        //crear evento
        public const string NuevoEvento = "Nuevo Evento";
        //comprar evento
        public const string Compra = "Pagado";
        //asiento ocupado
        public const string Ocupado = "OCUPADO";
        //cliente anonimo universal
        public const string AnonimoUniversal = "a@anonimo.com";
        //correo ventas
        public const string CorreoVentas = "ticknetventas@gmail.com";
        //contraseña ventas
        public const string ContraVentas = "Asdf1234!@";
        //crear una contraseña nueva aleatoria cuando un vendedor registra a un cliente
        public static string CreaPassword()
        {
            string caracteresValidosMinuscula = "abcdefghijkmnopqrstuvwxyz"; // 6 minusculas
            string caracteresValidosMayuscula = "ABCDEFGHJKLMNOPQRSTUVWXYZ"; // 1 mayuscula
            string caracteresNumeros = "0123456789"; // 2 numeros
            string caracteresSimbolos = "!@$#%&-+"; // 1 simbolo
            Random randNum = new Random((int)DateTime.Now.Ticks);
            char[] chars = new char[10];
            //primera letra mayuscula
            chars[0] = caracteresValidosMayuscula[randNum.Next(caracteresValidosMayuscula.Length)];
            //6 caracteres minuscula
            for (int i = 1; i < 7; i++)
            {
                chars[i] = caracteresValidosMinuscula[randNum.Next(caracteresValidosMinuscula.Length)];
            }
            //2 numeros
            for (int i = 7; i < 9; i++)
            {
                chars[i] = caracteresNumeros[randNum.Next(caracteresNumeros.Length)];
            }
            //1 simbolo
            chars[9] = caracteresSimbolos[randNum.Next(caracteresSimbolos.Length)];
            return new string(chars);
        }

    }


}