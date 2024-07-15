using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WSPlataforma.Entities.Reports.BindingModel;
using WSPlataforma.Entities.Reports.BindingModel.ReportEntities;
using WSPlataforma.Util;

namespace WSPlataforma.DA
{
    public class HtmlStringGenerator
    {
        private string GetStringDate(DateTime date, string mode)
        {
            string month = "";
            switch (date.Month)
            {
                case 1:
                    month = "enero";
                    break;
                case 2:
                    month = "febrero";
                    break;
                case 3:
                    month = "marzo";
                    break;
                case 4:
                    month = "abril";
                    break;
                case 5:
                    month = "mayo";
                    break;
                case 6:
                    month = "junio";
                    break;
                case 7:
                    month = "julio";
                    break;
                case 8:
                    month = "agosto";
                    break;
                case 9:
                    month = "septiembre";
                    break;
                case 10:
                    month = "octubre";
                    break;
                case 11:
                    month = "noviembre";
                    break;
                case 12:
                    month = "diciembre";
                    break;
            }
            if (mode == "dayName")
            {
                string dayName = "";
                switch ((int)date.DayOfWeek)
                {
                    case 1:
                        dayName = "Lunes";
                        break;
                    case 2:
                        dayName = "Martes";
                        break;
                    case 3:
                        dayName = "Miércoles";
                        break;
                    case 4:
                        dayName = "Jueves";
                        break;
                    case 5:
                        dayName = "Viernes";
                        break;
                    case 6:
                        dayName = "Sábado";
                        break;
                    case 7:
                        dayName = "Domingo";
                        break;
                }
                return dayName + ", " + date.Day + " de " + month + " de " + date.Year;
            }
            else
            {
                return "Lima, " + date.Day + " de " + month + " de " + date.Year;
            }

        }
        public string Slip_Cotizacion(Quotation data, string htmlTemplatePath)
        {
            var response = String.Empty;
            try
            {
                string miningText = data.IsMining == false ? "<p>* El contratante declara que la actividad no se realizará en mina.</p>" : "";
                string tables = String.Empty;
                string healthRiskList = String.Empty;
                string pensionRiskList = String.Empty;
                string conditions = String.Empty;
                string textForConjunct = String.Empty;
                string titleGloss = String.Empty;
                string conceptGloss = String.Empty;

                if (data.ProductId == ELog.obtainConfig("pensionKey"))
                {
                    for (int i = 0; i < data.RiskList.Count; i++)
                    {
                        if (data.RiskList[i].ProductId == ELog.obtainConfig("pensionKey")) pensionRiskList = pensionRiskList + "<tr><td>" + data.RiskList[i].Risk + "</td><td class='amount-field'>" + data.RiskList[i].Rate + "</td><td class='amount-field'>" + data.RiskList[i].Payroll + "</td><td class='amount-field'>" + data.RiskList[i].Premium + "</td></tr>";
                    }
                    tables = "<div style='width:100%; margin-top: 40px'>" +
                                     "<div class='product-title'>SCTR PENSIÓN</div>" +
                                     "<table border='1'>" +
                                    "                <thead>" +
                                    "                    <tr>" +
                                    "                        <th>RIESGO</th>" +
                                    "                        <th>TASA</th>" +
                                    "                        <th>PLANILLA MENSUAL</th>" +
                                    "                        <th>PRIMA NETA MENSUAL</th>" +
                                    "                    </tr>" +
                                    "                </thead>" +
                                    "                <tbody>" +
                                                     pensionRiskList +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial neta</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + data.RiskList[0].TotalNetPremium + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>IGV</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + data.RiskList[0].TotalPremimumIGV + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial total</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + data.RiskList[0].TotalGrossPremium + "</td>" +
                                    "                </tbody>" +
                                    "            </table>" +
                                     "</div>";
                    conditions = "" +
                                "<p><b>SCTR PENSIÓN</b></p>" +
                                "<p>a) Sobrevivencia:</p>" +
                                "<p>Los beneficiarios del asegurado recibirán una pensión, calculada sobre remuneración mensual y respetando la remuneración máximo asegurable vigente establecida por el Sistema Privado de Pensiones, en la siguiente proporción: </p>" +
                                "<ul>" +
                                "	<li>42% por Cónyuge o Concubino sin hijos beneficiarios</li>" +
                                "	<li>35% por Cónyuge o Concubino con hijos beneficiarios</li>" +
                                "	<li>14% para cada Hijo menor a 18 años</li>" +
                                "	<li>14% para cada Hijo inválido mayor a 18 años, incapacitado total y permanente de cualquier actividad laboral</li>" +
                                "	<li>" +
                                "		Hasta 14% para el Padre y/o la Madre, si queda remanente alguno y siempre que cumplan alguno de los siguientes requisitos:" +
                                "		<ul>" +
                                "			<li>Calificado como inválido total o parcialmente en proporción superior al 50%</li>" +
                                "			<li>Mayor de 60 años y que hayan dependido económicamente del asegurado</li>" +
                                "		</ul>" +
                                "	</li>" +
                                "</ul>" +
                                "<p>La sumatoria de los porcentajes de pensión no puede exceder el 100% de la remuneración mensual del asegurado.</p>" +
                                "<p>b) Pensión de Invalidez:</p>" +
                                "<p>" +
                                "	El asegurado recibirá una Pensión de Invalidez, calculada sobre remuneración mensual y respetando la remuneración máximo asegurable vigente establecida por el Sistema Privado de Pensiones, como consecuencia de un accidente de trabajo o enfermedad" +
                                "	profesional que diera origen a:" +
                                "</p>" +
                                "<ul>" +
                                "	<li>Invalidez Parcial Permanente Inferior al 50%: Se pagará una indemnización equivalente a 24 x 70% de la remuneración mensual, cuando el grado de menoscabo sea igual o superior al 20% y menos a 50%.</li>" +
                                "	<li>Invalidez Parcial Permanente: Se pagará una pensión vitalicia mensual equivalente al 50% de la remuneración mensual, cuando el grado de menoscabo sea igual o superior al 50% y menos a los 2/3.</li>" +
                                "	<li>" +
                                "		Invalidez Total Permanente: Se pagará una pensión vitalicia mensual equivalente al 70% de la remuneración mensual, cuando el grado de menoscabo sea igual o superior a los 2/3. La pensión será, como mínimo del 100% de la remuneración mensual," +
                                "		si como consecuencia del accidente de trabajo o enfermedad profesional, el asegurado calificado en condición de Invalidez Total y permanente quedará definitivamente incapacitado para realizar cualquier clase de trabajo remunerado y, además," +
                                "		requiera indispensablemente del auxilio de otra persona para movilizarse o para realizar las funciones esenciales para la vida." +
                                "	</li>" +
                                "</ul>" +
                                "<p>c) Gastos de Sepelio:</p>" +
                                "<p>Reembolso de los gastos de sepelio hasta el límite correspondiente al mes del fallecimiento, establecido por el Sistema Privado de Pensiones.</p>";
                }
                else if (data.ProductId == ELog.obtainConfig("saludKey"))
                {
                    for (int i = 0; i < data.RiskList.Count; i++)
                    {
                        if (data.RiskList[i].ProductId == ELog.obtainConfig("saludKey")) healthRiskList = healthRiskList + "<tr><td>" + data.RiskList[i].Risk + "</td><td class='amount-field'>" + data.RiskList[i].Rate + "</td><td class='amount-field'>" + data.RiskList[i].Payroll + "</td><td class='amount-field'>" + data.RiskList[i].Premium + "</td></tr>";
                    }
                    tables = "<div style='width:100%; margin-top: 200px'>" +
                                     "<div class='product-title'>SCTR SALUD</div>" +
                                     "<table border='1'>" +
                                    "                <thead>" +
                                    "                    <tr>" +
                                    "                        <th>RIESGO</th>" +
                                    "                        <th>TASA</th>" +
                                    "                        <th>PLANILLA MENSUAL</th>" +
                                    "                        <th>PRIMA NETA MENSUAL</th>" +
                                    "                    </tr>" +
                                    "                </thead>" +
                                    "                <tbody>" +
                                                     healthRiskList +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial neta</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + data.RiskList[0].TotalNetPremium + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>IGV</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + data.RiskList[0].TotalPremimumIGV + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial total</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + data.RiskList[0].TotalGrossPremium + "</td>" +
                                    "                    </tr>" +
                                    "                </tbody>" +
                                    "            </table>" +
                                     "</div>";
                    conditions = "" +
                                "<p><b>SCTR SALUD</b></p>" +
                                "<p>a) Asistencia y asesoramiento preventivo promocional en salud ocupacional a la ENTIDAD EMPLEADORA y a los ASEGURADOS;</p>" +
                                "<p>" +
                                "	b) Atención médica, farmacológica, hospitalaria y quirúrgica, cualquiera que fuere el nivel de complejidad; hasta la recuperación total del ASEGURADO o la declaración de una invalidez permanente total o parcial o fallecimiento. El ASEGURADO conserva" +
                                "	su derecho a ser atendido por el Seguro Social en Salud con posterioridad al alta o a la declaración de la invalidez permanente, de acuerdo con el Artículo 7 del Decreto Supremo N° 009-97-SA;" +
                                "</p>" +
                                "<p>c) Rehabilitación y readaptación laboral al ASEGURADO inválido bajo este seguro;</p>" +
                                "<p>d) Aparatos de prótesis y ortopédicos necesarios al ASEGURADO inválido bajo este seguro.</p>" +
                                "<p>Esta cobertura no comprende los subsidios económicos que son otorgados por cuenta del Seguro Social de Salud según lo previsto en los Artículos 15, 16 y 17 del Decreto Supremo.</p>";
                }
                else
                {
                    string HealthTotalNetPremium = "";
                    string HealthTotalPremimumIGV = "";
                    string HealthTotalGrossPremium = "";
                    string PensionTotalNetPremium = "";
                    string PensionTotalPremimumIGV = "";
                    string PensionTotalGrossPremium = "";

                    for (int i = 0; i < data.RiskList.Count; i++)
                    {
                        if (data.RiskList[i].ProductId == ELog.obtainConfig("saludKey"))
                        {
                            healthRiskList = healthRiskList + "<tr><td>" + data.RiskList[i].Risk + "</td><td class='amount-field'>" + data.RiskList[i].Rate + "</td><td class='amount-field'>" + data.RiskList[i].Payroll + "</td><td class='amount-field'>" + data.RiskList[i].Premium + "</td></tr>";
                            HealthTotalNetPremium = data.RiskList[i].TotalNetPremium;
                            HealthTotalPremimumIGV = data.RiskList[i].TotalPremimumIGV;
                            HealthTotalGrossPremium = data.RiskList[i].TotalGrossPremium;
                        }
                        else
                        {
                            pensionRiskList = pensionRiskList + "<tr><td>" + data.RiskList[i].Risk + "</td><td class='amount-field'>" + data.RiskList[i].Rate + "</td><td class='amount-field'>" + data.RiskList[i].Payroll + "</td><td class='amount-field'>" + data.RiskList[i].Premium + "</td></tr>";
                            PensionTotalNetPremium = data.RiskList[i].TotalNetPremium;
                            PensionTotalPremimumIGV = data.RiskList[i].TotalPremimumIGV;
                            PensionTotalGrossPremium = data.RiskList[i].TotalGrossPremium;
                        }
                    }
                    tables = "<div style='width:100%; margin-top: 40px'>" +
                                    "<div class='product-title'>SCTR SALUD</div>" +
                                    "<table border='1'>" +
                                    "                <thead>" +
                                    "                    <tr>" +
                                    "                        <th>RIESGO</th>" +
                                    "                        <th>TASA</th>" +
                                    "                        <th>PLANILLA MENSUAL</th>" +
                                    "                        <th>PRIMA NETA MENSUAL</th>" +
                                    "                    </tr>" +
                                    "                </thead>" +
                                    "                <tbody>" +
                                                     healthRiskList +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial neta</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + HealthTotalNetPremium + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>IGV</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + HealthTotalPremimumIGV + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial total</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + HealthTotalGrossPremium + "</td>" +
                                    "                    </tr>" +
                                    "                </tbody>" +
                                    "            </table>" +
                                     "</div>" +
                                     "<div style='width:100%; margin-top: 40px'>" +
                                     "<div class='product-title'>SCTR PENSIÓN</div>" +
                                     "<table border='1'>" +
                                    "                <thead>" +
                                    "                    <tr>" +
                                    "                        <th>RIESGO</th>" +
                                    "                        <th>TASA</th>" +
                                    "                        <th>PLANILLA MENSUAL</th>" +
                                    "                        <th>PRIMA NETA MENSUAL</th>" +
                                    "                    </tr>" +
                                    "                </thead>" +
                                    "                <tbody>" +
                                                     pensionRiskList +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial neta</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + PensionTotalNetPremium + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>IGV</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + PensionTotalPremimumIGV + "</td>" +
                                    "                    </tr>" +
                                    "                    <tr class='white-field'>" +
                                    "                        <td colspan='2'>Prima comercial total</td>" +
                                    "                        <td style='text-align:right'>: S/.</td>" +
                                    "                        <td style='text-align:right'>" + PensionTotalGrossPremium + "</td>" +
                                    "                    </tr>" +
                                    "                </tbody>" +
                                    "            </table>" +
                                     "</div>";


                    conditions = "" +
                                "<p><b>SCTR SALUD</b></p>" +
                                "<p>a) Asistencia y asesoramiento preventivo promocional en salud ocupacional a la ENTIDAD EMPLEADORA y a los ASEGURADOS;</p>" +
                                "<p>" +
                                "	b) Atención médica, farmacológica, hospitalaria y quirúrgica, cualquiera que fuere el nivel de complejidad; hasta la recuperación total del ASEGURADO o la declaración de una invalidez permanente total o parcial o fallecimiento. El ASEGURADO conserva" +
                                "	su derecho a ser atendido por el Seguro Social en Salud con posterioridad al alta o a la declaración de la invalidez permanente, de acuerdo con el Artículo 7 del Decreto Supremo N° 009-97-SA;" +
                                "</p>" +
                                "<p>c) Rehabilitación y readaptación laboral al ASEGURADO inválido bajo este seguro;</p>" +
                                "<p>d) Aparatos de prótesis y ortopédicos necesarios al ASEGURADO inválido bajo este seguro.</p>" +
                                "<p>Esta cobertura no comprende los subsidios económicos que son otorgados por cuenta del Seguro Social de Salud según lo previsto en los Artículos 15, 16 y 17 del Decreto Supremo.</p>" +
                                 "<p><b>SCTR PENSIÓN</b></p>" +
                                "<p>a) Sobrevivencia:</p>" +
                                "<p>Los beneficiarios del asegurado recibirán una pensión, calculada sobre remuneración mensual y respetando la remuneración máximo asegurable vigente establecida por el Sistema Privado de Pensiones, en la siguiente proporción: </p>" +
                                "<ul>" +
                                "	<li>42% por Cónyuge o Concubino sin hijos beneficiarios</li>" +
                                "	<li>35% por Cónyuge o Concubino con hijos beneficiarios</li>" +
                                "	<li>14% para cada Hijo menor a 18 años</li>" +
                                "	<li>14% para cada Hijo inválido mayor a 18 años, incapacitado total y permanente de cualquier actividad laboral</li>" +
                                "	<li>" +
                                "		Hasta 14% para el Padre y/o la Madre, si queda remanente alguno y siempre que cumplan alguno de los siguientes requisitos:" +
                                "		<ul>" +
                                "			<li>Calificado como inválido total o parcialmente en proporción superior al 50%</li>" +
                                "			<li>Mayor de 60 años y que hayan dependido económicamente del asegurado</li>" +
                                "		</ul>" +
                                "	</li>" +
                                "</ul>" +
                                "<p>La sumatoria de los porcentajes de pensión no puede exceder el 100% de la remuneración mensual del asegurado.</p>" +
                                "<p>b) Pensión de Invalidez:</p>" +
                                "<p>" +
                                "	El asegurado recibirá una Pensión de Invalidez, calculada sobre remuneración mensual y respetando la remuneración máximo asegurable vigente establecida por el Sistema Privado de Pensiones, como consecuencia de un accidente de trabajo o enfermedad" +
                                "	profesional que diera origen a:" +
                                "</p>" +
                                "<ul>" +
                                "	<li>Invalidez Parcial Permanente Inferior al 50%: Se pagará una indemnización equivalente a 24 x 70% de la remuneración mensual, cuando el grado de menoscabo sea igual o superior al 20% y menos a 50%.</li>" +
                                "	<li>Invalidez Parcial Permanente: Se pagará una pensión vitalicia mensual equivalente al 50% de la remuneración mensual, cuando el grado de menoscabo sea igual o superior al 50% y menos a los 2/3.</li>" +
                                "	<li>" +
                                "		Invalidez Total Permanente: Se pagará una pensión vitalicia mensual equivalente al 70% de la remuneración mensual, cuando el grado de menoscabo sea igual o superior a los 2/3. La pensión será, como mínimo del 100% de la remuneración mensual," +
                                "		si como consecuencia del accidente de trabajo o enfermedad profesional, el asegurado calificado en condición de Invalidez Total y permanente quedará definitivamente incapacitado para realizar cualquier clase de trabajo remunerado y, además," +
                                "		requiera indispensablemente del auxilio de otra persona para movilizarse o para realizar las funciones esenciales para la vida." +
                                "	</li>" +
                                "</ul>" +
                                "<p>c) Gastos de Sepelio:</p>" +
                                "<p>Reembolso de los gastos de sepelio hasta el límite correspondiente al mes del fallecimiento, establecido por el Sistema Privado de Pensiones.</p>";
                    textForConjunct = "<p>Las presentes condiciones son válidas siempre que se confirme la colocación de ambos productos. En caso se desee colocar solo uno de los productos, se deberá solicitar una nueva cotización.</p>";
                }

                if (!String.IsNullOrEmpty(data.Gloss))
                {
                    titleGloss = "<h3 style='text-decoration: underline'>IMPORTANTE</h3>";
                    conceptGloss = "<p>" + data.Gloss + "</p>";
                }

                string htmlString = File.ReadAllText(htmlTemplatePath);

                htmlString = htmlString
                    .Replace("[miningText]", miningText)
                    .Replace("[data.ProductName]", data.ProductName)
                    .Replace("[data.Contractor]", data.Contractor)
                    .Replace("[data.InsuredQuantity]", data.InsuredQuantity)
                    .Replace("[data.Payroll]", data.Payroll)
                    .Replace("[data.EconomicActivity]", data.EconomicActivity)
                    .Replace("[data.RenovationMinimunPremium]", data.RenovationMinimunPremium)
                    .Replace("[data.EndorsementMinimunPremium]", data.EndorsementMinimunPremium)
                    .Replace("[tables]", tables)
                    .Replace("[conditions]", conditions)
                    .Replace("[textForConjunct]", textForConjunct)
                    .Replace("[titleImportant]", titleGloss)
                    .Replace("[data.Gloss]", conceptGloss)
                    .Replace("[data.OperationDate]", GetStringDate(data.OperationDate, "place"));
                response = ReplaceCharacters(htmlString);
            }
            catch (Exception ex)
            {
                LogControl.save("Slip_Cotizacion", ex.ToString(), "3");
            }

            return response;

        }
        public string Contrato_Salud(Contrato data, string htmlTemplatePath)
        {
            var response = String.Empty;
            try
            {
                string tempHeader = "<div><img src='" + data.Sanitas.IconPath + "' width='275'></div>";
                string insuredList = "";
                string excludedList = "";
                string includedList = "";
                if (data.InsuredList != null && data.InsuredList.Count > 0)
                {
                    for (int i = 0; i < data.InsuredList.Count; i++)
                    {
                        int index = i + 1;
                        if (data.TransactionType == "5") //Neteo
                        {
                            if (data.InsuredList[i].Type == "I") includedList = includedList + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                            else excludedList = excludedList + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                        }
                        else if (data.TransactionType == "4")   //Renovación
                        {
                            if (data.InsuredList[i].Type == "R" || data.InsuredList[i].Type == "I") insuredList = insuredList + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                        }
                        else insuredList = insuredList + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                    }
                }
                string riskDetailList = "";
                if (data.Policy.RiskDetailList != null && data.Policy.RiskDetailList.Count > 0)
                {
                    for (int i = 0; i < data.Policy.RiskDetailList.Count; i++)
                    {
                        riskDetailList = riskDetailList + "" +
                            "    <tr>" +
                            "        <td style='width:25%;'>" + data.Policy.RiskDetailList[i].RiskType + "</td>" +
                            "        <td style='width:5%;'>:</td>" +
                            "        <td style='width:15%;'>" + data.Policy.RiskDetailList[i].WorkersCount + "</td>" +
                            "        <td style='text-align:right;width:10%;'>Tasa :</td>" +
                            "        <td style='text-align:right;width:10%;'>" + data.Policy.RiskDetailList[i].Rate + "</td>" +
                            "        <td style='text-align:right;width:20%;'>Planilla Mensual :</td>" +
                            "        <td style='text-align:right;width:15%;'>" + data.Policy.RiskDetailList[i].PayRoll + "</td>" +
                            "    </tr>";
                    }
                }

                string title = "";
                string insuredSection = "";
                switch (data.TransactionType)
                {
                    case "1":
                        title = "    <p><b>CONTRATO DE</b></p>";
                        insuredSection = "" +
                                        "<p style='text-decoration: underline;'><b>DATOS DE EMISIÓN</b></p><br>" +
                                        "<p>N° de Contrato : " + data.Policy.Number + "</p>" +
                                        "<p>Contratante : " + data.Contractor.Name + "</p>" +
                                        "<p>Vigencia : " + data.VigencyStartDate.ToString("dd/MM/yyyy") + " al " + data.VigencyEndDate.ToString("dd/MM/yyyy") + "</p>" +
                                        "<p>Sede : " + data.Contractor.Location.Description + " - " + data.Contractor.Location.District + " - " + data.Contractor.Location.Province + " - " + data.Contractor.Location.Department + "</p><br>" +
                                        "<table border='1' style='width: 100%; text-align: left; line-height: 10pt;'>" +
                                        "    <tr>" +
                                        "        <th style='padding:5px'>Nro.</th>" +
                                        "        <th style='padding:5px'>Nombres</th>" +
                                        "        <th style='padding:5px'>Apellido Paterno</th>" +
                                        "        <th style='padding:5px'>Apellido Materno</th>" +
                                        "        <th style='padding:5px'>Nro. Documento</th>" +
                                        "        <th style='padding:5px'>Sede</th>" +
                                        "    </tr>" +
                                        "    <tbody>" +
                                        insuredList +
                                        "</tbody>" +
                                        "</table>";

                        break;
                    case "2":
                        title = "    <p><b>CONSTANCIA DE INCLUSIÓN A CONTRATO DE</b></p>";
                        insuredSection = "" +
                                        "<p style='text-decoration: underline;'><b>DATOS DE INCLUSIÓN</b></p><br>" +
                                        "<p>N° de Contrato : " + data.Policy.Number + "</p>" +
                                        "<p>Contratante : " + data.Contractor.Name + "</p>" +
                                        "<p>Vigencia : " + data.VigencyStartDate.ToString("dd/MM/yyyy") + " al " + data.VigencyEndDate.ToString("dd/MM/yyyy") + "</p>" +
                                        "<p>Sede : " + data.Contractor.Location.Description + " - " + data.Contractor.Location.District + " - " + data.Contractor.Location.Province + " - " + data.Contractor.Location.Department + "</p><br>" +
                                        "<table border='1' style='width: 100%; text-align: left; line-height: 10pt;'>" +
                                        "    <tr>" +
                                        "        <th style='padding:5px'>Nro.</th>" +
                                        "        <th style='padding:5px'>Nombres</th>" +
                                        "        <th style='padding:5px'>Apellido Paterno</th>" +
                                        "        <th style='padding:5px'>Apellido Materno</th>" +
                                        "        <th style='padding:5px'>Nro. Documento</th>" +
                                        "        <th style='padding:5px'>Sede</th>" +
                                        "    </tr>" +
                                        "    <tbody>" +
                                        insuredList +
                                        "</tbody>" +
                                        "</table>";
                        break;
                    case "3":
                        title = "    <p><b>CONSTANCIA DE EXCLUSIÓN A CONTRATO DE</b></p>";
                        insuredSection = "" +
                                        "<p style='text-decoration: underline;'><b>DATOS DE EXCLUSIÓN</b></p><br>" +
                                        "<p>N° de Contrato : " + data.Policy.Number + "</p>" +
                                        "<p>Contratante : " + data.Contractor.Name + "</p>" +
                                        "<p>Vigencia : " + data.VigencyStartDate.ToString("dd/MM/yyyy") + " al " + data.VigencyEndDate.ToString("dd/MM/yyyy") + "</p>" +
                                        "<p>Sede : " + data.Contractor.Location.Description + " - " + data.Contractor.Location.District + " - " + data.Contractor.Location.Province + " - " + data.Contractor.Location.Department + "</p><br>" +
                                        "<table border='1' style='width: 100%; text-align: left; line-height: 10pt;'>" +
                                        "    <tr>" +
                                        "        <th style='padding:5px'>Nro.</th>" +
                                        "        <th style='padding:5px'>Nombres</th>" +
                                        "        <th style='padding:5px'>Apellido Paterno</th>" +
                                        "        <th style='padding:5px'>Apellido Materno</th>" +
                                        "        <th style='padding:5px'>Nro. Documento</th>" +
                                        "        <th style='padding:5px'>Sede</th>" +
                                        "    </tr>" +
                                        "    <tbody>" +
                                        insuredList +
                                        "</tbody>" +
                                        "</table>";
                        break;
                    case "4":
                        title = "    <p><b>RENOVACIÓN DE CONTRATO</b></p>";
                        insuredSection = "" +
                                        "<p style='text-decoration: underline;'><b>DATOS DE RENOVACIÓN</b></p><br>" +
                                        "<p>N° de Contrato : " + data.Policy.Number + "</p>" +
                                        "<p>Contratante : " + data.Contractor.Name + "</p>" +
                                        "<p>Vigencia : " + data.VigencyStartDate.ToString("dd/MM/yyyy") + " al " + data.VigencyEndDate.ToString("dd/MM/yyyy") + "</p>" +
                                        "<p>Sede : " + data.Contractor.Location.Description + " - " + data.Contractor.Location.District + " - " + data.Contractor.Location.Province + " - " + data.Contractor.Location.Department + "</p><br>" +
                                        "<table border='1' style='width: 100%; text-align: left; line-height: 10pt;'>" +
                                        "    <tr>" +
                                        "        <th style='padding:5px'>Nro.</th>" +
                                        "        <th style='padding:5px'>Nombres</th>" +
                                        "        <th style='padding:5px'>Apellido Paterno</th>" +
                                        "        <th style='padding:5px'>Apellido Materno</th>" +
                                        "        <th style='padding:5px'>Nro. Documento</th>" +
                                        "        <th style='padding:5px'>Sede</th>" +
                                        "    </tr>" +
                                        "    <tbody>" +
                                        insuredList +
                                        "</tbody>" +
                                        "</table>";
                        break;
                    case "5":
                        title = "    <p><b>CONSTANCIA DE INCLUSIÓN/EXCLUSIÓN A CONTRATO DE</b></p>";
                        insuredSection = "" +
                                        "<p style='text-decoration: underline;'><b>DATOS DE INCLUSIÓN</b></p><br>" +
                                        "<p>N° de Contrato : " + data.Policy.Number + "</p>" +
                                        "<p>Contratante : " + data.Contractor.Name + "</p>" +
                                        "<p>Vigencia : " + data.VigencyStartDate.ToString("dd/MM/yyyy") + " al " + data.VigencyEndDate.ToString("dd/MM/yyyy") + "</p>" +
                                        "<p>Sede : " + data.Contractor.Location.Description + " - " + data.Contractor.Location.District + " - " + data.Contractor.Location.Province + " - " + data.Contractor.Location.Department + "</p><br>" +
                                        "<table border='1' style='width: 100%; text-align: left; line-height: 10pt;'>" +
                                        "    <tr>" +
                                        "        <th style='padding:5px'>Nro.</th>" +
                                        "        <th style='padding:5px'>Nombres</th>" +
                                        "        <th style='padding:5px'>Apellido Paterno</th>" +
                                        "        <th style='padding:5px'>Apellido Materno</th>" +
                                        "        <th style='padding:5px'>Nro. Documento</th>" +
                                        "        <th style='padding:5px'>Sede</th>" +
                                        "    </tr>" +
                                        "    <tbody>" +
                                        includedList +
                                        "</tbody>" +
                                        "</table>" +
                                        "<br>" +
                                        "<br>" +
                                        "<br>" +
                                        "<p style='text-decoration: underline;'><b>DATOS DE EXCLUSIÓN</b></p><br>" +
                                        "<p>N° de Contrato : " + data.Policy.Number + "</p>" +
                                        "<p>Contratante : " + data.Contractor.Name + "</p>" +
                                        "<p>Vigencia : " + data.VigencyStartDate.ToString("dd/MM/yyyy") + " al " + data.VigencyEndDate.ToString("dd/MM/yyyy") + "</p>" +
                                        "<p>Sede : " + data.Contractor.Location.Description + " - " + data.Contractor.Location.District + " - " + data.Contractor.Location.Province + " - " + data.Contractor.Location.Department + "</p><br>" +
                                        "<table border='1' style='width: 100%; text-align: left; line-height: 10pt;'>" +
                                        "    <tr>" +
                                        "        <th style='padding:5px'>Nro.</th>" +
                                        "        <th style='padding:5px'>Nombres</th>" +
                                        "        <th style='padding:5px'>Apellido Paterno</th>" +
                                        "        <th style='padding:5px'>Apellido Materno</th>" +
                                        "        <th style='padding:5px'>Nro. Documento</th>" +
                                        "        <th style='padding:5px'>Sede</th>" +
                                        "    </tr>" +
                                        "    <tbody>" +
                                        excludedList +
                                        "</tbody>" +
                                        "</table>";
                        break;
                }


                string htmlString = File.ReadAllText(htmlTemplatePath);

                htmlString = htmlString
                    .Replace("[data.Policy.Number]", data.Policy.Number)
                    .Replace("[data.Contractor.Name]", data.Contractor.Name)
                    .Replace("[data.Contractor.DocumentType]", data.Contractor.DocumentType)
                    .Replace("[data.Contractor.DocumentNumber]", data.Contractor.DocumentNumber)
                    .Replace("[data.Contractor.Location.EconomicActivity]", data.Contractor.Location.EconomicActivity)
                    .Replace("[data.Contractor.Address]", data.Contractor.Address)
                    .Replace("[data.Contractor.District]", data.Contractor.District)
                    .Replace("[data.Contractor.Province]", data.Contractor.Province)
                    .Replace("[data.Contractor.Department]", data.Contractor.Department)
                    .Replace("[riskDetailList]", riskDetailList)
                    .Replace("[data.Policy.TotalNetPremium]", data.Policy.TotalNetPremium)
                    .Replace("[data.Policy.TotalPremiumIGV]", data.Policy.TotalPremiumIGV)
                    .Replace("[data.Policy.TotalGrossPremium]", data.Policy.TotalGrossPremium)
                    .Replace("[data.VigencyStartDate]", data.VigencyStartDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.VigencyEndDate]", data.VigencyEndDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.OperationDate]", data.OperationDate.ToString("dd/MM/yyyy HH:mm"))
                    .Replace("[data.Contractor.Location.Description]", data.Contractor.Location.Description)
                    .Replace("[data.Contractor.EconomicActiviy]", data.Contractor.EconomicActiviy)
                    .Replace("[data.Contractor.Location.District]", data.Contractor.Location.District)
                    .Replace("[data.Contractor.Location.Province]", data.Contractor.Location.Province)
                    .Replace("[data.Contractor.Location.Department]", data.Contractor.Location.Department)
                    .Replace("[data.OperationDate.String]", GetStringDate(data.OperationDate, "dayName"))
                    .Replace("[data.Sanitas.SignaturePath]", data.Sanitas.SignaturePath)
                    .Replace("[insuredSection]", insuredSection)
                    .Replace("[title]", title);

                response = ReplaceCharacters(htmlString);
            }
            catch (Exception ex)
            {
                LogControl.save("Contrato_Salud", ex.ToString(), "3");
            }

            return response;
        }

        public string Constancia(Constancia data, string productId, string IOMode, string htmlTemplatePath)
        {
            var response = String.Empty;

            try
            {
                string miningText = data.IsMining ? "" : "<br /><p>*El contratante ha declarado que la actividad a realizar no se realizará en mina.</p>";
                string insuredString = String.Empty;
                if (data.TransactionType == "5")
                {
                    if (data.InsuredList != null && data.InsuredList.Count > 0)
                    {
                        for (int i = 0; i < data.InsuredList.Count; i++)
                        {
                            int index = i + 1;
                            if (data.InsuredList[i].Type == IOMode) insuredString = insuredString + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                        }
                    }
                }
                else if (data.TransactionType == "4")
                {
                    if (data.InsuredList != null && data.InsuredList.Count > 0)
                    {
                        for (int i = 0; i < data.InsuredList.Count; i++)
                        {
                            int index = i + 1;
                            if (data.InsuredList[i].Type == "I" || data.InsuredList[i].Type == "R") insuredString = insuredString + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                        }
                    }
                }
                else
                {
                    if (data.InsuredList != null && data.InsuredList.Count > 0)
                    {
                        for (int i = 0; i < data.InsuredList.Count; i++)
                        {
                            int index = i + 1;
                            insuredString = insuredString + "<tr><td>" + index + "</td><td>" + data.InsuredList[i].FirstName + "</td><td>" + data.InsuredList[i].PaternalLastName + "</td><td>" + data.InsuredList[i].MaternalLastName + "</td><td>" + data.InsuredList[i].DocumentType + " - " + data.InsuredList[i].DocumentNumber + "</td><td>" + data.InsuredList[i].LocationName + "</td></tr>";
                        }
                    }
                }


                string firstSubtitle = String.Empty;
                string secondSubtitle = String.Empty;
                string signatureHtml = String.Empty;
                string rmaConcept = String.Empty;

                if (productId == ELog.obtainConfig("saludKey"))
                {
                    firstSubtitle = "SEGURO COMPLEMENTARIO DE TRABAJO DE RIESGO SALUD";
                    secondSubtitle = "<th style='width: 100%; text-align: left; border-right: 0;'> Contrato SCTR - Salud N°: " + data.HealthPolicy.Number + "</th>";
                    signatureHtml = "" +
                                    "		<td style='align-content: center; width: 50%; border-color: white'>" +
                                    "			<div style='margin: auto;height: 300px;width: 250px;'>" +
                                    "				<img src=" + data.Sanitas.SignaturePath + " style='width:200px;margin-bottom:10px'>" +
                                    "				<div style='border-top: 1px solid black; text-align: center;padding-top: 10px;font-weight: 600'>" +
                                    "					" + data.Sanitas.Name +
                                    "				</div>" +
                                    "			</div>" +
                                    "		</td>";
                }
                else if (productId == ELog.obtainConfig("pensionKey"))
                {
                    firstSubtitle = "SEGURO COMPLEMENTARIO DE TRABAJO DE RIESGO PENSIÓN";
                    secondSubtitle += "<th style='width: 50%; text-align: left; border-right: 0;'>Póliza SCTR - Pensión N°: " + data.PensionPolicy.Number + "</th>";
                    secondSubtitle += "<th style='width: 50%; text-align: right; border-left: 0;'>RMA*: " + data.RmaPension + "</th>";
                    signatureHtml = "" +
                                    "		<td style='align-content: center; width: 50%; border-color: white'>" +
                                    "			<div style='margin: auto;height: 300px;width: 250px;'>" +
                                    "				<img src=" + data.Protecta.SignaturePath + " style='width:200px;margin-bottom:10px'>" +
                                    "				<div style='border-top: 1px solid black; text-align: center;padding-top: 10px;font-weight: 600'>" +
                                    "					" + data.Protecta.Name +
                                    "				</div>" +
                                    "			</div>" +
                                    "		</td>";
                    rmaConcept = "* Remuneración máxima asegurable";
                }
                else
                {
                    firstSubtitle = "SEGURO COMPLEMENTARIO DE TRABAJO DE RIESGO SALUD Y PENSIÓN";
                    secondSubtitle += "<th style='width: 50%; text-align: left; border-right: 0;'>Contrato SCTR - Salud N°: " + data.HealthPolicy.Number + "</th>";
                    secondSubtitle += "<th style='width: 50%; text-align: left; border-right: 0;'>Póliza SCTR - Pensión N°:" + data.PensionPolicy.Number + "<span style='float:right; border-left: 0;'>RMA*: " + data.RmaPension + "</span></th>";
                    signatureHtml = "" +
                                    "		<td style='align-content: center; width: 50%; border-color: white'>" +
                                    "			<div style='margin: auto;height: 300px;width: 250px;'>" +
                                    "				<img src=" + data.Sanitas.SignaturePath + " style='width:200px;height:150px;margin-bottom:10px'>" +
                                    "				<div style='border-top: 1px solid black; text-align: center;padding-top: 10px;font-weight: 600'>" +
                                    "					" + data.Sanitas.Name +
                                    "				</div>" +
                                    "			</div>" +
                                    "		</td>" +
                                    "		<td style='align-content: center; width: 50%; border-color: white'>" +
                                    "			<div style='margin: auto;height: 300px;width: 250px;'>" +
                                    "				<img src=" + data.Protecta.SignaturePath + " style='width:200px;height:150px;margin-bottom:10px'>" +
                                    "				<div style='border-top: 1px solid black; text-align: center;padding-top: 10px;font-weight: 600'>" +
                                    "					" + data.Protecta.Name +
                                    "				</div>" +
                                    "			</div>" +
                                    "		</td>";
                    rmaConcept = "* Remuneración máxima asegurable";
                }
                string title = String.Empty;
                string firstParagraph = String.Empty;

                if (data.TransactionType == "3" || IOMode == "E")
                {
                    title = "CONSTANCIA DE EXCLUSIÓN N°: " + data.ProofNumber;
                    firstParagraph = "" +
                        "Por medio del presente documento dejamos constancia que, los asegurados detallados en líneas posteriores, han sido excluidos de la póliza de salud y SCTR pensión  desde el " + data.VigencyStartDate.ToString("dd/MM/yyyy") + ", conforme a lo solicitado por el contratante.";
                }
                else if (data.TransactionType == "2" || IOMode == "I")
                {
                    title = "CONSTANCIA DE INCLUSIÓN N°: " + data.ProofNumber;
                    firstParagraph = "" +
                    "Por medio del presente dejamos constancia que los asegurados " +
                    "detallados líneas abajo, conforme al Decreto Supremo 003-98-SA, se encuentran amparados bajo la cobertura de salud " +
                    "de trabajo de riesgo.";

                }
                else
                {
                    title = "CONSTANCIA N°: " + data.ProofNumber;
                    firstParagraph = "" +
                    "Por medio del presente dejamos constancia que los asegurados " +
                    "detallados líneas abajo, conforme al Decreto Supremo 003-98-SA, se encuentran amparados bajo la cobertura de salud " +
                    "de trabajo de riesgo.";
                }


                string htmlString = File.ReadAllText(htmlTemplatePath);

                htmlString = htmlString
                    .Replace("[data.Contractor.Name]", data.Contractor.Name)
                    .Replace("[data.VigencyStartDate]", data.VigencyStartDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.VigencyEndDate]", data.VigencyEndDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.Contractor.EconomicActiviy]", data.Contractor.EconomicActiviy)
                    .Replace("[data.Contractor.Location.Description]", data.Contractor.Location.Description)
                    .Replace("[data.Contractor.Location.District]", data.Contractor.Location.District)
                    .Replace("[data.Contractor.Location.Province]", data.Contractor.Location.Province)
                    .Replace("[data.Contractor.Location.Department]", data.Contractor.Location.Department)
                    .Replace("[data.Contractor.Name]", data.Contractor.Name)
                    .Replace("[data.OperationDate]", GetStringDate(data.OperationDate, "place"))
                    .Replace("[signatureHtml]", signatureHtml)
                    .Replace("[firstSubtitle]", firstSubtitle)
                    .Replace("[firstParagraph]", firstParagraph)
                    .Replace("[secondSubtitle]", secondSubtitle)
                    .Replace("[insuredString]", insuredString)
                    .Replace("[rmaConcept]", rmaConcept)
                    .Replace("[title]", title)
                    .Replace("[miningText]", miningText);

                response = ReplaceCharacters(htmlString);
            }
            catch (Exception ex)
            {
                LogControl.save("Constancia", ex.ToString(), "3");
            }

            return response;
        }

        public string Proforma(Proforma data, string htmlTemplatePath)
        {
            var response = String.Empty;
            try
            {
                string paymentModeList = "";
                if (data.PaymentModeList != null && data.PaymentModeList.Count > 0)
                {
                    for (int i = 0; i < data.PaymentModeList.Count; i++)
                    {
                        if (data.Policy.ProductTypeId == ELog.obtainConfig("saludKey"))
                        {
                            paymentModeList = paymentModeList +
                              "            <tr>" +
                                "                <td style='font-size: 6pt;text-decoration:underline;'>" + data.PaymentModeList[i].ShortName + " : </td>" +
                                "                <td style='font-size: 6pt;'>" + data.PaymentModeList[i].LongName + "</td>" +
                                "            </tr>";
                        }

                        if (data.Policy.ProductTypeId != ELog.obtainConfig("saludKey"))
                        {
                            paymentModeList =
                                "            <tr>" +
                                "                <td style='font-size: 6pt;text-decoration:underline;'>" + data.PaymentModeList[0].ShortName + " : </td>" +
                                "                <td style='font-size: 6pt;'>" + data.PaymentModeList[0].LongName + "</td>" +
                                "            </tr>";
                        }

                    }
                }
                string productName = "";
                if (data.Policy.ProductTypeId == ELog.obtainConfig("saludKey"))
                {
                    productName = "SCTR SALUD";
                }
                else productName = "SCTR PENSIÓN";

                string htmlString = File.ReadAllText(htmlTemplatePath);
                htmlString = htmlString
                    .Replace("[data.Protecta.IconPath]", data.Protecta.IconPath)
                    .Replace("[data.Protecta.Name]", data.Protecta.Name)
                    .Replace("[data.Protecta.Address]", data.Protecta.Address)
                    .Replace("[data.Protecta.Department]", data.Protecta.Department)
                    .Replace("[data.Protecta.Province]", data.Protecta.Province)
                    .Replace("[data.Protecta.District]", data.Protecta.District)
                    .Replace("[data.Protecta.RUC]", data.Protecta.RUC)
                    .Replace("[data.OperationDate]", data.OperationDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.ProformaCode]", data.ProformaCode)
                    .Replace("[data.ProformaExpirationDate]", data.ProformaExpirationDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.Policy.Number]", data.Policy.Number)
                    .Replace("[data.Protecta.Department]", data.Protecta.Department)
                    .Replace("[data.Protecta.District]", data.Protecta.District)
                    .Replace("[productName]", productName)
                    .Replace("[data.Policy.VigencyStartDate]", data.Policy.VigencyStartDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.Policy.VigencyEndDate]", data.Policy.VigencyEndDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.Contractor.Name]", data.Contractor.Name)
                    .Replace("[data.Contractor.DocumentNumber]", data.Contractor.DocumentNumber)
                    .Replace("[data.Contractor.Address]", data.Contractor.Address)
                    .Replace("[data.Contractor.Department]", data.Contractor.Department)
                    .Replace("[data.Contractor.Province]", data.Contractor.Province)
                    .Replace("[data.Contractor.District]", data.Contractor.District)
                    .Replace("[data.Contractor.ZipCode]", data.Contractor.ZipCode)
                    .Replace("[data.Contractor.Phone]", data.Contractor.Phone)
                    .Replace("[data.Policy.TotalNetPremium]", data.Policy.TotalNetPremium)
                    .Replace("[data.Policy.TotalPremiumIGV]", data.Policy.TotalPremiumIGV)
                    .Replace("[data.Policy.TotalGrossPremium]", data.Policy.TotalGrossPremium)
                    .Replace("[paymentModeList]", paymentModeList)
                    .Replace("[data.Protecta.WEB]", data.Protecta.WEB);
                response = ReplaceCharacters(htmlString);
            }
            catch (Exception ex)
            {
                LogControl.save("Proforma", ex.ToString(), "3");
            }
            return response;
        }

        public string Solicitud_Pension(Solicitud_Pension data, string htmlTemplatePath)
        {
            var response = String.Empty;
            try
            {
                string tempHeader = "<div><img src='" + data.Protecta.IconPath + "' width='275'> <span style='float: right; margin-top: 30px; font-size: 8pt; font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;'>Fecha: " + data.OperationDate.ToString("dd/MM/yyyy") + "</span></div>";
                string protectaPhoneList = "";
                if (data.Protecta.PhoneList != null && data.Protecta.PhoneList.Count > 0)
                {
                    for (int i = 0; i < data.Protecta.PhoneList.Count; i++)
                    {
                        protectaPhoneList = protectaPhoneList + "<div>" + data.Protecta.PhoneList[i] + "</div>";
                    }
                }


                string insuranceCoverages = "";
                if (data.InsuranceCoverages != null && data.InsuranceCoverages.Count > 0)
                {
                    for (int i = 0; i < data.InsuranceCoverages.Count; i++)
                    {
                        insuranceCoverages = insuranceCoverages + "<tr><td colspan='3'>" + data.InsuranceCoverages[i] + "</td></tr>";
                    }
                    insuranceCoverages = insuranceCoverages + "<tr><td colspan='3'><span>Beneficio Máximo</span></td></tr><tr><td colspan='3'>" + data.TextB + "</td></tr>";
                }
                string insuranceExclusions = "";
                if (data.InsuranceExclusions != null && data.InsuranceExclusions.Count > 0)
                {
                    for (int i = 0; i < data.InsuranceExclusions.Count; i++)
                    {
                        insuranceExclusions = insuranceExclusions + "<li>" + data.InsuranceExclusions[i] + "</li>";
                    }
                }
                string riskDetailList = "";
                if (data.Policy.RiskDetailList != null && data.Policy.RiskDetailList.Count > 0)
                {
                    for (int i = 0; i < data.Policy.RiskDetailList.Count; i++)
                    {
                        riskDetailList = riskDetailList + "<tr style='width: 100%'><td style='width: 50%;'><span>" + data.Policy.RiskDetailList[i].RiskType + " : </span></td><td style='width: 25%; text-align:center;'>" + data.Policy.RiskDetailList[i].WorkersCount + "</td><td style='width: 25%; text-align: center;'>" + data.Policy.RiskDetailList[i].Rate + "</td></tr>";
                    }
                }
                string insuranceCoveragePlaceList = "";
                if (data.InsuranceCoveragePlaces != null && data.InsuranceCoveragePlaces.Count > 0)
                {
                    for (int i = 0; i < data.InsuranceCoveragePlaces.Count; i++)
                    {
                        insuranceCoveragePlaceList = insuranceCoveragePlaceList + "" +
                            "<li>" +
                              data.InsuranceCoveragePlaces[i] +
                            "</li>";
                    }
                }

                string htmlString = File.ReadAllText(htmlTemplatePath);
                htmlString = htmlString
                    .Replace("[data.Policy.Number]", data.Policy.Number)
                    .Replace("[data.Protecta.Name]", data.Protecta.Name)
                    .Replace("[data.Protecta.RUC]", data.Protecta.RUC)
                    .Replace("[data.Protecta.Address]", data.Protecta.Address)
                    .Replace("[protectaPhoneList]", protectaPhoneList)
                    .Replace("[data.Protecta.Email]", data.Protecta.Email)
                    .Replace("[data.Contractor.Name]", data.Contractor.Name)
                    .Replace("[data.Contractor.DocumentType]", data.Contractor.DocumentType)
                    .Replace("[data.Contractor.DocumentNumber]", data.Contractor.DocumentNumber)
                    .Replace("[data.Contractor.Address]", data.Contractor.Address)
                    .Replace("[data.Contractor.Phone]", data.Contractor.Phone)
                    .Replace("[data.Contractor.EconomicActiviy]", data.Contractor.EconomicActiviy)
                    .Replace("[data.Contractor.CIIU]", data.Contractor.CIIU)
                    .Replace("[data.InsuranceManager]", data.InsuranceManager)
                    .Replace("[data.InsuranceManagerPosition]", data.InsuranceManagerPosition)
                    .Replace("[data.InsuranceManagerEmail]", data.InsuranceManagerEmail)
                    .Replace("[data.Broker.Name]", data.Broker.Name)
                    .Replace("[data.Broker.DocumentType]", data.Broker.DocumentType)
                    .Replace("[data.Broker.DocumentNumber]", data.Broker.DocumentNumber)
                    .Replace("[data.Broker.Address]", data.Broker.Address)
                    .Replace("[data.Broker.Phone]", data.Broker.Phone)
                    .Replace("[data.Broker.SBSCode]", data.Broker.SBSCode)
                    .Replace("[data.Broker.Email]", data.Broker.Email)
                    .Replace("[data.WorkersTotalNumber]", data.WorkersTotalNumber)
                    .Replace("[data.Contractor.Location.EconomicActivity]", data.Contractor.Location.EconomicActivity)
                    .Replace("[data.PayrollAmount]", data.PayrollAmount)
                    .Replace("[data.Policy.VigencyStartDate]", data.Policy.VigencyStartDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.Policy.VigencyEndDate]", data.Policy.VigencyEndDate.ToString("dd/MM/yyyy"))
                    .Replace("[data.Policy.TotalGrossPremium]", data.Policy.TotalGrossPremium)
                    .Replace("[riskDetailList]", riskDetailList)
                    .Replace("[data.PremiumPaymentMode]", data.PremiumPaymentMode)
                    .Replace("[data.PremiumPaymentPlace]", data.PremiumPaymentPlace)
                    .Replace("[insuranceCoverages]", insuranceCoverages)
                    .Replace("[insuranceExclusions]", insuranceExclusions)
                    .Replace("[insuranceCoveragePlaceList]", insuranceCoveragePlaceList);
                response = ReplaceCharacters(htmlString);
            }
            catch (Exception ex)
            {
                LogControl.save("Solicitud_Pension", ex.ToString(), "3");
            }
            return response;

        }

        public string Condiciones_Particulares_Pension(Condiciones_Particulares_Pension data, string htmlTemplatePath)
        {
            var response = String.Empty;
            try
            {
                string tempHeader = "<div><img src='" + data.Protecta.IconPath + "' width='275'> <span style='float: right; margin-top: 30px; font-size: 8pt; font-family: 'Gill Sans', 'Gill Sans MT', Calibri, 'Trebuchet MS', sans-serif;'>Fecha: " + data.OperationDate.ToString("dd/MM/yyyy") + "</span></div>";
                string riskTableContent = "";
                for (int i = 0; i < data.Policy.RiskDetailList.Count; i++)
                {
                    if (i == 0) riskTableContent = riskTableContent + "<tr><td>" + data.Contractor.Location.EconomicActivity + "</td><td>" + data.Policy.RiskDetailList[i].RiskType + "</td><td style='text-align:right'>" + data.Policy.RiskDetailList[i].WorkersCount + "</td><td style='text-align:right'>" + data.Policy.RiskDetailList[i].PayRoll + "</td><td style='text-align:right'>" + data.Policy.RiskDetailList[i].Rate + "</td><td></td><td></td></tr>";
                    else riskTableContent = riskTableContent + "<tr><td></td><td>" + data.Policy.RiskDetailList[i].RiskType + "</td><td style='text-align:right'>" + data.Policy.RiskDetailList[i].WorkersCount + "</td><td style='text-align:right'>" + data.Policy.RiskDetailList[i].PayRoll + "</td><td style='text-align:right'>" + data.Policy.RiskDetailList[i].Rate + "</td><td></td><td></td></tr>";
                }

                string insuranceCoverages = "";
                if (data.InsuranceCoverageList != null && data.InsuranceCoverageList.Count > 0)
                {
                    for (int i = 0; i < data.InsuranceCoverageList.Count; i++)
                    {
                        insuranceCoverages = insuranceCoverages + "<li>" + data.InsuranceCoverageList[i] + "</li>";
                    }
                }
                string protectaPhoneList = "";
                if (data.Protecta.PhoneList != null && data.Protecta.PhoneList.Count > 0)
                {
                    for (int i = 0; i < data.Protecta.PhoneList.Count; i++)
                    {
                        string style = (i > 0) ? style = " style='margin-left:5px;'" : style = "";
                        protectaPhoneList = protectaPhoneList + "<div" + style + ">" + data.Protecta.PhoneList[i] + "</div>";
                    }
                }

                string htmlString = File.ReadAllText(htmlTemplatePath);

                htmlString = htmlString
                   .Replace("[data.Policy.Number]", data.Policy.Number)
                   .Replace("[data.Protecta.Name]", data.Protecta.Name)
                   .Replace("[data.Protecta.Address]", data.Protecta.Address)
                   .Replace("[data.Protecta.District]", data.Protecta.District)
                   .Replace("[data.Protecta.Province]", data.Protecta.Province)
                   .Replace("[data.Protecta.Department]", data.Protecta.Department)
                   .Replace("[data.Protecta.RUC]", data.Protecta.RUC)
                   .Replace("[protectaPhoneList]", protectaPhoneList)
                   .Replace("[data.Protecta.Email]", data.Protecta.Email)
                   .Replace("[data.Contractor.Name]", data.Contractor.Name)
                   .Replace("[data.Contractor.Address]", data.Contractor.Address)
                   .Replace("[data.Policy.VigencyStartDate]", data.Policy.VigencyStartDate.ToString("dd/MM/yyyy"))
                   .Replace("[data.Policy.VigencyEndDate]", data.Policy.VigencyEndDate.ToString("dd/MM/yyyy"))
                   .Replace("[data.TextA]", data.TextA)
                   .Replace("[insuranceCoverages]", insuranceCoverages)
                   .Replace("[data.TextB]", data.TextB)
                   .Replace("[data.Contractor.Location.Description]", data.Contractor.Location.Description)
                   .Replace("[riskTableContent]", riskTableContent)
                   .Replace("[data.Policy.TotalNetPremium]", data.Policy.TotalNetPremium)
                   .Replace("[data.Policy.TotalPremiumIGV]", data.Policy.TotalPremiumIGV)
                   .Replace("[data.Policy.TotalGrossPremium]", data.Policy.TotalGrossPremium)
                   .Replace("[data.PaymentMode]", data.PaymentMode)
                   .Replace("[data.PaymentPlace]", data.PaymentPlace)
                   .Replace("[data.Broker.Name]", data.Broker.Name)
                   .Replace("[data.Broker.SBSCode]", data.Broker.SBSCode)
                   .Replace("[data.Broker.Commission]", data.Broker.Commission)
                   .Replace("[data.OperationDate]", GetStringDate(data.OperationDate, "place"))
                   .Replace("[data.OperationDate(justdate)]", data.OperationDate.ToString("dd/MM/yyyy"));

                response = ReplaceCharacters(htmlString);
            }
            catch (Exception ex)
            {
                LogControl.save("Condiciones_Particulares_Pension", ex.ToString(), "3");
            }
            return response;
        }

        private string ReplaceCharacters(string html)
        {
            string chars = ELog.obtainConfig("CharSpec");

            foreach (var oneChar in chars.Split(','))
            {
                html = html.Replace(oneChar, HttpContext.Current.Server.HtmlEncode(oneChar));
            }

            return html;
        }
    }

}
