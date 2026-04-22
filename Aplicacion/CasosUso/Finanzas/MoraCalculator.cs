using System;

namespace FinancieraSoluciones.Application.CasosUso.Finanzas
{
    public static class MoraCalculator
    {
        public static decimal Calcular(
            string tipo,
            DateTime fechaFicha,
            DateTime hoy,
            decimal moraDiaria,
            decimal moraSemanal,
            decimal moraMensual,
            int diasGraciaDiaria,
            int diasGraciaSemanal,
            int diasGraciaMensual,
            decimal topeMoraDiaria,
            decimal topeMoraSemanal,
            decimal topeMoraMensual)
        {
            var diasAtraso = (hoy.Date - fechaFicha.Date).Days;
            if (diasAtraso <= 0) return 0;

            var t = (tipo ?? string.Empty).Trim().ToLower();
            if (t == "diario")
            {
                var gracia = diasGraciaDiaria < 0 ? 0 : diasGraciaDiaria;
                if (diasAtraso <= gracia) return 0;
                
                var diasSujetosAMora = diasAtraso - gracia;
                var vecesAAplicar = (topeMoraDiaria > 0 && diasSujetosAMora > (int)topeMoraDiaria) 
                    ? (int)topeMoraDiaria 
                    : diasSujetosAMora;

                return vecesAAplicar * moraDiaria;
            }

            if (t == "semanal")
            {
                var gracia = diasGraciaSemanal < 0 ? 0 : diasGraciaSemanal;
                if (diasAtraso <= gracia) return 0;

                var diasSujetosAMora = diasAtraso - gracia;
                var vecesAAplicar = (topeMoraSemanal > 0 && diasSujetosAMora > (int)topeMoraSemanal) 
                    ? (int)topeMoraSemanal 
                    : diasSujetosAMora;

                return vecesAAplicar * moraSemanal;
            }

            if (t == "mensual")
            {
                var gracia = diasGraciaMensual < 0 ? 0 : diasGraciaMensual;
                if (diasAtraso <= gracia) return 0;

                var diasSujetosAMora = diasAtraso - gracia;
                var vecesAAplicar = (topeMoraMensual > 0 && diasSujetosAMora > (int)topeMoraMensual) 
                    ? (int)topeMoraMensual 
                    : diasSujetosAMora;

                return vecesAAplicar * moraMensual;
            }

            return 0;
        }
    }
}

