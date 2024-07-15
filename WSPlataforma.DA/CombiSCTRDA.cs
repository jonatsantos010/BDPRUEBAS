using Oracle.DataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.PolicyModel.BindingModel;
using WSPlataforma.Entities.PolicyModel.ViewModel;
using WSPlataforma.Entities.QuotationModel.BindingModel;
using quotationVM = WSPlataforma.Entities.QuotationModel.ViewModel;
using WSPlataforma.Util;
using WSPlataforma.Util.PrintPolicyUtility;
using WSPlataforma.Entities.QuotationModel;
using System.Diagnostics;
using SpreadsheetLight;
using WSPlataforma.Entities.AjustedAmountsModel.BindingModel;
using WSPlataforma.Entities.AjustedAmountsModel.ViewModel;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using Newtonsoft.Json;
using DCPoliza = WSPlataforma.Entities.QuotationModel.ViewModel;

namespace WSPlataforma.DA
{
    public class CombiSCTRDA : ConnectionBase
    {
        private SharedMethods sharedMethods = new SharedMethods();
        QuotationDA QuotationDA = new QuotationDA();

        public ErrorCode Msj_TecnicsSCTR(HisDetRenovSCTR data)
        {

            List<OracleParameter> parameter = new List<OracleParameter>();
            var response = new HisDetRenovSCTR();
            var errorCode = new ErrorCode();
            var VAR_A = "EL GASTO PROPUESTO ES MAYOR AL GASTO POR ASESORÍA, SE DERIVA AL ÁREA TÉCNICA.";
            var VAR_B = "EL TOTAL DE TRABAJADORES SUPERA LOS MIL, SE DERIVA AL ÁREA TÉCNICA.";
            var VAR_C = "LA PRIMA MINIMA PROPUESTA ES MENOR A LA ENVIADA, SE DERIVA AL ÁREA TÉCNICA.";
            var VAR_D = "LA TASA PROPUESTA ES MENOR A LA NETA, SE DERIVA AL ÁREA TÉCNICA.";
            var VAR_E = "ACTIVIDAD DE RIESGO SELECCIONADA, SE DERIVA AL ÁREA TÉCNICA.";
            var VAR_F = "LA ACTIVIDAD SE ENCUENTRA DELIMITADA";

            string storedProcedureName = ProcedureName.pkg_TecnicaSCTR + ".UDP_TODO";
            try
            {

                if (data.P_LISTATASAPENSION_LENGTH != 0)
                {

                    //A
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //C
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //E
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA != 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_NACT_MINA == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA == 0)
                    {
                        if (data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AB
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AC
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //AD
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000
                        && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AE
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //AF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0
                        && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BC
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BD
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //BE
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0
                        && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //CD
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //CE
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 &&
                        data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {

                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //CF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //DE
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //DF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //EF
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA != 0)
                    {
                        if (data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //A B C
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B E

                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C E
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A C F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A D E

                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000
                        && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B C D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C E
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D E
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //C D E
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //C D F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA == 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //C E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_LISTATASASALUD_LENGTH == 0 &&
                        data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {

                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //D E F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //A B C D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C E
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D E
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D E
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A C D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A D E F

                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D E
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D E F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //C D E F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //A B C D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D E F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D E F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_WORKER < 1000 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D E F
                    if (data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 || data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1
                         || data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 || data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C D E F
                    if (data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3 && data.P_COM_PENSION_PRO > 23
                        && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }
                }

                if (data.P_LISTATASASALUD_LENGTH != 0)
                {

                    //A
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //C
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_SDELIMITER == 0 &&
                        data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //E
                    if (data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA != 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_NACT_MINA == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //F
                    if (data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_LISTATASASALUD_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA == 0)
                    {
                        if (data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AB
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AC
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //AD
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000
                        && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0
                        && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //AE
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BC
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BD
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //BE
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0
                        && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //CD
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //CE
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 &&
                        data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {

                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //CF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 &&
                        data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //DE
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //DF
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //EF
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA != 0)
                    {
                        if (data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //A B C
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B E

                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //A C D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C E
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A C F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //A D E

                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000
                        && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }



                    //B C D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C E
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B C F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //B D E
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //C D E
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_SDELIMITER == 0
                            && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //C D F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA == 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_LISTATASAPENSION_LENGTH == 0 &&
                        data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {

                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //D E F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C D
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //A B C E
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D E
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D E
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A C D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //B C D E
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_NACT_MINA == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A D E F

                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D E F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //C D E F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //A B C D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_LENGTH == 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D E F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D E F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_WORKER < 1000 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D E F
                    if (data.P_LISTATASAPENSION_LENGTH == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 || data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1
                         || data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 || data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //A B C D E F
                    if (data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_COM_PENSION_PRO > 23
                        && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }
                }

                if (data.P_LISTATASASALUD_LENGTH != 0 && data.P_LISTATASAPENSION_LENGTH != 0)
                {

                    //A
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_SDELIMITER == 0)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //C
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //D
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //E
                    if (data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA != 0)
                    {
                        if (data.P_NACT_MINA == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //F
                    if (data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA == 0)
                    {
                        if (data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AB
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AC
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //AD
                    if (data.P_NACT_MINA == 0 && data.P_WORKER < 1000 && data.P_SDELIMITER == 0
                        && data.P_PRIMA_MIN_PENSION_PRO == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_COM_PENSION_PRO > 23 && data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //AE
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //AF
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0
                        && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //BC
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BD
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //BE
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                       && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //BF
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0
                        && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //CD
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //CE
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {

                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //CF
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //DE
                    if (data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //DF
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //EF
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0
                        && data.P_NACT_MINA != 0)
                    {
                        if (data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }




                    //A B C
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D
                    if (data.P_NACT_MINA == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B E

                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_PRIMA_MIN_PENSION_PRO == 0)
                    {
                        if (data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D
                    if (data.P_NACT_MINA == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C E
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A D E

                    if (data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A D F
                    if (data.P_NACT_MINA == 0 && data.P_WORKER < 1000
                        && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_COM_PENSION_PRO > 23 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }


                    //B C D
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C E
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D E
                    if (data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D F
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //C D E
                    if (data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_SDELIMITER == 0
                            && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //C D F
                    if (data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA == 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_COM_PENSION_PRO < 23
                        && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {

                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //D E F
                    if (data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C D
                    if (data.P_NACT_MINA == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C E
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 && data.P_SDELIMITER == 0
                        && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_E, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D E
                    if (data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D E
                    if (data.P_WORKER < 1000 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_NACT_MINA == 0 && data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D F
                    if (data.P_NACT_MINA == 0 && data.P_LISTATASASALUD_LENGTH == 0 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A C D F
                    if (data.P_NACT_MINA == 0 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A D E F

                    if (data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }


                    //B C D E
                    if (data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 0)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D F
                    if (data.P_NACT_MINA == 0 && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                       && data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //B D E F
                    if (data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //C D E F
                    if (data.P_COM_PENSION_PRO < 23 && data.P_WORKER < 1000 && data.P_PRIMA_MIN_PENSION_PRO != 0
                            && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }

                    }

                    //A B C D F
                    if (data.P_NACT_MINA == 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C E F
                    if (data.P_LISTATASASALUD_PLANPROP0 == 0 && data.P_LISTATASASALUD_PLANPROP1 == 0 && data.P_LISTATASASALUD_PLANPROP2 == 0 && data.P_LISTATASASALUD_PLANPROP3 == 0 &&
                        data.P_LISTATASAPENSION_PLANPROP0 == 0 && data.P_LISTATASAPENSION_PLANPROP1 == 0 && data.P_LISTATASAPENSION_PLANPROP2 == 0 && data.P_LISTATASAPENSION_PLANPROP3 == 0
                        && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }

                    //A B D E F
                    if (data.P_PRIMA_MIN_PENSION_PRO == 0 && data.P_NACT_MINA != 0 && data.P_COM_PENSION_PRO > 23 && data.P_WORKER >= 1001 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A C D E F
                    if (data.P_WORKER < 1000 && data.P_NACT_MINA != 0 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA == 1 && data.P_COM_PENSION_PRO > 23 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //B C D E F
                    if (data.P_COM_PENSION_PRO < 23 && data.P_PRIMA_MIN_PENSION_PRO != 0 && data.P_NACT_MINA != 0 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        if (data.P_LISTATASASALUD_PLANPROP0 > 0 && data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASAPENSION_PLANPROP0 > 0 && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 ||
                            data.P_LISTATASASALUD_PLANPROP1 > 0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASAPENSION_PLANPROP1 > 0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 ||
                            data.P_LISTATASASALUD_PLANPROP2 > 0 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASAPENSION_PLANPROP2 > 0 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 ||
                            data.P_LISTATASASALUD_PLANPROP3 > 0 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_LISTATASAPENSION_PLANPROP3 > 0 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3)
                        {
                            parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                            parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                        }
                    }

                    //A B C D E F
                    if (data.P_LISTATASASALUD_PLANPROP0 < data.RATE_SALUD0 && data.P_LISTATASASALUD_PLANPROP1 < data.RATE_SALUD1 && data.P_LISTATASASALUD_PLANPROP2 < data.RATE_SALUD2 && data.P_LISTATASASALUD_PLANPROP3 < data.RATE_SALUD3 && data.P_COM_PENSION_PRO > 23
                        && data.P_LISTATASAPENSION_PLANPROP0 < data.RATE_PENSION0 && data.P_LISTATASAPENSION_PLANPROP1 < data.RATE_PENSION1 && data.P_LISTATASAPENSION_PLANPROP2 < data.RATE_PENSION2 && data.P_LISTATASAPENSION_PLANPROP3 < data.RATE_PENSION3 && data.P_WORKER >= 1001 && data.P_PRIMA_MIN_PENSION_PRO < 80 && data.P_NACT_MINA == 1 && data.P_SDELIMITER == 1)
                    {
                        parameter.Add(new OracleParameter("P_MENSAJE", OracleDbType.Varchar2, VAR_A + " / " + VAR_B + " / " + VAR_C + " / " + VAR_D + " / " + VAR_E + " / " + VAR_F, ParameterDirection.Input));
                        parameter.Add(new OracleParameter("P_NID_COTIZACION", OracleDbType.Int32, data.P_NID_COTIZACION, ParameterDirection.Input));
                    }
                }


                //OUTPUT
                OracleParameter P_COD_ERR = new OracleParameter("P_COD_ERR", OracleDbType.Int32, errorCode.P_COD_ERR, ParameterDirection.Output);
                OracleParameter P_MESSAGE = new OracleParameter("P_MESSAGE", OracleDbType.Varchar2, 9000, errorCode.P_MESSAGE, ParameterDirection.Output);

                parameter.Add(P_COD_ERR);
                parameter.Add(P_MESSAGE);

                OracleDataReader odr = (OracleDataReader)this.ExecuteByStoredProcedureVT(storedProcedureName, parameter);

                ELog.CloseConnection(odr);
            }
            catch (Exception ex)
            {
                errorCode.P_COD_ERR = 1;
                errorCode.P_MESSAGE = ex.ToString();
                LogControl.save("Msj_TecnicsSCTR", ex.ToString(), "3");
            }

            return errorCode;
        }

    }
}