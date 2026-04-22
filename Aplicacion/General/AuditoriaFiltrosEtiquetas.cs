namespace FinancieraSoluciones.Application.General
{
    internal static class AuditoriaFiltrosEtiquetas
    {
        public static string EtiquetaAccion(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;
            return valor switch
            {
                "CobroFicha" => "Cobro en ficha de crédito",

                "ReversarMovimientoCaja" => "Reverso de movimiento de caja",
                "ReestructurarCredito" => "Reestructuración de crédito",
                "CrearLiquidacionCobranza" => "Creación de liquidación de cobranza",
                "ResetPasswordAdmin" => "Restablecimiento de contraseña (administrador)",
                "RealizarCorteCaja" => "Corte de caja",
                "CerrarDia" => "Cierre de día",
                "CambiarPassword" => "Cambio de contraseña",
                "RestablecerPassword" => "Restablecimiento de contraseña",
                "SolicitarRecuperacionPassword" => "Solicitud de recuperación de contraseña",
                "LoginFallido" => "Inicio de sesión fallido",
                "LoginBloqueado" => "Cuenta bloqueada por intentos de acceso",
                "LoginRequiereCambioPassword" => "Inicio de sesión: debe cambiar contraseña",
                "LoginPasswordExpirada" => "Inicio de sesión: contraseña expirada",
                "LoginExitoso" => "Inicio de sesión exitoso",
                "MultaFicha" => "Multa o penalización en ficha",
                "CondonarInteresFicha" => "Condonación de interés (ficha)",
                "CondonarInteresMonto" => "Condonación de interés (monto)",
                "CrearCliente" => "Alta de cliente",
                "ActualizarCliente" => "Actualización de cliente",
                "EliminarCliente" => "Eliminación de cliente",
                "CrearCredito" => "Alta de crédito",
                "ConfirmarLiquidacionCobranza" => "Confirmación de liquidación de cobranza",
                "RechazarLiquidacionCobranza" => "Rechazo de liquidación de cobranza",
                _ => valor,
            };
        }

        public static string EtiquetaEntidadTipo(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor)) return valor;
            return valor switch
            {
                "Cliente" => "Cliente",
                "Credito" => "Crédito",
                "MovimientoCaja" => "Movimiento de caja",
                "LiquidacionCobranza" => "Liquidación de cobranza",
                "Usuario" => "Usuario y acceso",
                "CorteCaja" => "Corte de caja",
                "CierreDiario" => "Cierre diario",
                _ => valor,
            };
        }
    }
}
