﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Este código fue generado por una herramienta.
//     Versión de runtime:4.0.30319.42000
//
//     Los cambios en este archivo podrían causar un comportamiento incorrecto y se perderán si
//     se vuelve a generar el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WSPlataforma.DA.PEWSCryptoReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="PEWSCryptoReference.WSCryptoSoap")]
    public interface WSCryptoSoap {
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento plainText del espacio de nombres http://tempuri.org/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/EncryptText", ReplyAction="*")]
        WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponse EncryptText(WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/EncryptText", ReplyAction="*")]
        System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponse> EncryptTextAsync(WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento encryptText del espacio de nombres http://tempuri.org/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DecryptText", ReplyAction="*")]
        WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponse DecryptText(WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/DecryptText", ReplyAction="*")]
        System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponse> DecryptTextAsync(WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest request);
        
        // CODEGEN: Se está generando un contrato de mensaje, ya que el nombre de elemento plainText del espacio de nombres http://tempuri.org/ no está marcado para aceptar valores nil.
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Signer", ReplyAction="*")]
        WSPlataforma.DA.PEWSCryptoReference.SignerResponse Signer(WSPlataforma.DA.PEWSCryptoReference.SignerRequest request);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/Signer", ReplyAction="*")]
        System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.SignerResponse> SignerAsync(WSPlataforma.DA.PEWSCryptoReference.SignerRequest request);
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class EncryptTextRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="EncryptText", Namespace="http://tempuri.org/", Order=0)]
        public WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequestBody Body;
        
        public EncryptTextRequest() {
        }
        
        public EncryptTextRequest(WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class EncryptTextRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string plainText;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public byte[] publicKey;
        
        public EncryptTextRequestBody() {
        }
        
        public EncryptTextRequestBody(string plainText, byte[] publicKey) {
            this.plainText = plainText;
            this.publicKey = publicKey;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class EncryptTextResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="EncryptTextResponse", Namespace="http://tempuri.org/", Order=0)]
        public WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponseBody Body;
        
        public EncryptTextResponse() {
        }
        
        public EncryptTextResponse(WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class EncryptTextResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string EncryptTextResult;
        
        public EncryptTextResponseBody() {
        }
        
        public EncryptTextResponseBody(string EncryptTextResult) {
            this.EncryptTextResult = EncryptTextResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DecryptTextRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DecryptText", Namespace="http://tempuri.org/", Order=0)]
        public WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequestBody Body;
        
        public DecryptTextRequest() {
        }
        
        public DecryptTextRequest(WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class DecryptTextRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string encryptText;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public byte[] privateKey;
        
        public DecryptTextRequestBody() {
        }
        
        public DecryptTextRequestBody(string encryptText, byte[] privateKey) {
            this.encryptText = encryptText;
            this.privateKey = privateKey;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class DecryptTextResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="DecryptTextResponse", Namespace="http://tempuri.org/", Order=0)]
        public WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponseBody Body;
        
        public DecryptTextResponse() {
        }
        
        public DecryptTextResponse(WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class DecryptTextResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string DecryptTextResult;
        
        public DecryptTextResponseBody() {
        }
        
        public DecryptTextResponseBody(string DecryptTextResult) {
            this.DecryptTextResult = DecryptTextResult;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SignerRequest {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="Signer", Namespace="http://tempuri.org/", Order=0)]
        public WSPlataforma.DA.PEWSCryptoReference.SignerRequestBody Body;
        
        public SignerRequest() {
        }
        
        public SignerRequest(WSPlataforma.DA.PEWSCryptoReference.SignerRequestBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SignerRequestBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string plainText;
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=1)]
        public byte[] privateKey;
        
        public SignerRequestBody() {
        }
        
        public SignerRequestBody(string plainText, byte[] privateKey) {
            this.plainText = plainText;
            this.privateKey = privateKey;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.ServiceModel.MessageContractAttribute(IsWrapped=false)]
    public partial class SignerResponse {
        
        [System.ServiceModel.MessageBodyMemberAttribute(Name="SignerResponse", Namespace="http://tempuri.org/", Order=0)]
        public WSPlataforma.DA.PEWSCryptoReference.SignerResponseBody Body;
        
        public SignerResponse() {
        }
        
        public SignerResponse(WSPlataforma.DA.PEWSCryptoReference.SignerResponseBody Body) {
            this.Body = Body;
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [System.Runtime.Serialization.DataContractAttribute(Namespace="http://tempuri.org/")]
    public partial class SignerResponseBody {
        
        [System.Runtime.Serialization.DataMemberAttribute(EmitDefaultValue=false, Order=0)]
        public string SignerResult;
        
        public SignerResponseBody() {
        }
        
        public SignerResponseBody(string SignerResult) {
            this.SignerResult = SignerResult;
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WSCryptoSoapChannel : WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WSCryptoSoapClient : System.ServiceModel.ClientBase<WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap>, WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap {

        /// <summary>
        /// Implement this partial method to configure the service endpoint.
        /// </summary>
        /// <param name="serviceEndpoint">The endpoint to configure</param>
        /// <param name="clientCredentials">The client credentials</param>
        static partial void ConfigureEndpoint(System.ServiceModel.Description.ServiceEndpoint serviceEndpoint, System.ServiceModel.Description.ClientCredentials clientCredentials);

        public WSCryptoSoapClient(EndpointConfiguration endpointConfiguration) :
                base(WSCryptoSoapClient.GetBindingForEndpoint(endpointConfiguration), WSCryptoSoapClient.GetEndpointAddress(endpointConfiguration))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public WSCryptoSoapClient(EndpointConfiguration endpointConfiguration, string remoteAddress) :
                base(WSCryptoSoapClient.GetBindingForEndpoint(endpointConfiguration), new System.ServiceModel.EndpointAddress(remoteAddress))
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public WSCryptoSoapClient(EndpointConfiguration endpointConfiguration, System.ServiceModel.EndpointAddress remoteAddress) :
                base(WSCryptoSoapClient.GetBindingForEndpoint(endpointConfiguration), remoteAddress)
        {
            this.Endpoint.Name = endpointConfiguration.ToString();
            ConfigureEndpoint(this.Endpoint, this.ClientCredentials);
        }

        public WSCryptoSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) :
                base(binding, remoteAddress)
        {
        }

        public System.Threading.Tasks.Task<PEWSCryptoReference.EncryptTextResponse> EncryptTextAsync(PEWSCryptoReference.EncryptTextRequest request)
        {
            return base.Channel.EncryptTextAsync(request);
        }

        public System.Threading.Tasks.Task<PEWSCryptoReference.DecryptTextResponse> DecryptTextAsync(PEWSCryptoReference.DecryptTextRequest request)
        {
            return base.Channel.DecryptTextAsync(request);
        }

        public System.Threading.Tasks.Task<PEWSCryptoReference.SignerResponse> SignerAsync(PEWSCryptoReference.SignerRequest request)
        {
            return base.Channel.SignerAsync(request);
        }

        public virtual System.Threading.Tasks.Task OpenAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndOpen));
        }

        public virtual System.Threading.Tasks.Task CloseAsync()
        {
            return System.Threading.Tasks.Task.Factory.FromAsync(((System.ServiceModel.ICommunicationObject)(this)).BeginClose(null, null), new System.Action<System.IAsyncResult>(((System.ServiceModel.ICommunicationObject)(this)).EndClose));
        }

        private static System.ServiceModel.Channels.Binding GetBindingForEndpoint(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WSCryptoSoap))
            {
                System.ServiceModel.BasicHttpBinding result = new System.ServiceModel.BasicHttpBinding();
                result.MaxBufferSize = int.MaxValue;
                result.ReaderQuotas = System.Xml.XmlDictionaryReaderQuotas.Max;
                result.MaxReceivedMessageSize = int.MaxValue;
                result.AllowCookies = true;
                return result;
            }
            if ((endpointConfiguration == EndpointConfiguration.WSCryptoSoap12))
            {
                System.ServiceModel.Channels.CustomBinding result = new System.ServiceModel.Channels.CustomBinding();
                System.ServiceModel.Channels.TextMessageEncodingBindingElement textBindingElement = new System.ServiceModel.Channels.TextMessageEncodingBindingElement();
                textBindingElement.MessageVersion = System.ServiceModel.Channels.MessageVersion.CreateVersion(System.ServiceModel.EnvelopeVersion.Soap12, System.ServiceModel.Channels.AddressingVersion.None);
                result.Elements.Add(textBindingElement);
                System.ServiceModel.Channels.HttpTransportBindingElement httpBindingElement = new System.ServiceModel.Channels.HttpTransportBindingElement();
                httpBindingElement.AllowCookies = true;
                httpBindingElement.MaxBufferSize = int.MaxValue;
                httpBindingElement.MaxReceivedMessageSize = int.MaxValue;
                result.Elements.Add(httpBindingElement);
                return result;
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        private static System.ServiceModel.EndpointAddress GetEndpointAddress(EndpointConfiguration endpointConfiguration)
        {
            if ((endpointConfiguration == EndpointConfiguration.WSCryptoSoap))
            {
                return new System.ServiceModel.EndpointAddress("http://pre1a.pagoefectivo.pe/PagoEfectivoWSCrypto/WSCrypto.asmx"); //desarrollo
                //return new System.ServiceModel.EndpointAddress("http://cip.pagoefectivo.pe/PagoEfectivoWSCrypto/WSCrypto.asmx"); //produccion
            }
            if ((endpointConfiguration == EndpointConfiguration.WSCryptoSoap12))
            {
                return new System.ServiceModel.EndpointAddress("http://pre1a.pagoefectivo.pe/PagoEfectivoWSCrypto/WSCrypto.asmx"); //desarrollo
                //return new System.ServiceModel.EndpointAddress("http://cip.pagoefectivo.pe/PagoEfectivoWSCrypto/WSCrypto.asmx"); //produccion
            }
            throw new System.InvalidOperationException(string.Format("Could not find endpoint with name \'{0}\'.", endpointConfiguration));
        }

        public enum EndpointConfiguration
        {

            WSCryptoSoap,

            WSCryptoSoap12,
        }

        //public WSCryptoSoapClient() {
        //}
        
        //public WSCryptoSoapClient(string endpointConfigurationName) : 
        //        base(endpointConfigurationName) {
        //}
        
        //public WSCryptoSoapClient(string endpointConfigurationName, string remoteAddress) : 
        //        base(endpointConfigurationName, remoteAddress) {
        //}
        
        //public WSCryptoSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
        //        base(endpointConfigurationName, remoteAddress) {
        //}
        
        //public WSCryptoSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
        //        base(binding, remoteAddress) {
        //}
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponse WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap.EncryptText(WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest request) {
            return base.Channel.EncryptText(request);
        }
        
        public string EncryptText(string plainText, byte[] publicKey) {
            WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest inValue = new WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest();
            inValue.Body = new WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequestBody();
            inValue.Body.plainText = plainText;
            inValue.Body.publicKey = publicKey;
            WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponse retVal = ((WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap)(this)).EncryptText(inValue);
            return retVal.Body.EncryptTextResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponse> WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap.EncryptTextAsync(WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest request) {
            return base.Channel.EncryptTextAsync(request);
        }
        
        public System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.EncryptTextResponse> EncryptTextAsync(string plainText, byte[] publicKey) {
            WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest inValue = new WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequest();
            inValue.Body = new WSPlataforma.DA.PEWSCryptoReference.EncryptTextRequestBody();
            inValue.Body.plainText = plainText;
            inValue.Body.publicKey = publicKey;
            return ((WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap)(this)).EncryptTextAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponse WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap.DecryptText(WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest request) {
            return base.Channel.DecryptText(request);
        }
        
        public string DecryptText(string encryptText, byte[] privateKey) {
            WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest inValue = new WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest();
            inValue.Body = new WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequestBody();
            inValue.Body.encryptText = encryptText;
            inValue.Body.privateKey = privateKey;
            WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponse retVal = ((WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap)(this)).DecryptText(inValue);
            return retVal.Body.DecryptTextResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponse> WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap.DecryptTextAsync(WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest request) {
            return base.Channel.DecryptTextAsync(request);
        }
        
        public System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.DecryptTextResponse> DecryptTextAsync(string encryptText, byte[] privateKey) {
            WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest inValue = new WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequest();
            inValue.Body = new WSPlataforma.DA.PEWSCryptoReference.DecryptTextRequestBody();
            inValue.Body.encryptText = encryptText;
            inValue.Body.privateKey = privateKey;
            return ((WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap)(this)).DecryptTextAsync(inValue);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        WSPlataforma.DA.PEWSCryptoReference.SignerResponse WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap.Signer(WSPlataforma.DA.PEWSCryptoReference.SignerRequest request) {
            return base.Channel.Signer(request);
        }
        
        public string Signer(string plainText, byte[] privateKey) {
            WSPlataforma.DA.PEWSCryptoReference.SignerRequest inValue = new WSPlataforma.DA.PEWSCryptoReference.SignerRequest();
            inValue.Body = new WSPlataforma.DA.PEWSCryptoReference.SignerRequestBody();
            inValue.Body.plainText = plainText;
            inValue.Body.privateKey = privateKey;
            WSPlataforma.DA.PEWSCryptoReference.SignerResponse retVal = ((WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap)(this)).Signer(inValue);
            return retVal.Body.SignerResult;
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.SignerResponse> WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap.SignerAsync(WSPlataforma.DA.PEWSCryptoReference.SignerRequest request) {
            return base.Channel.SignerAsync(request);
        }
        
        public System.Threading.Tasks.Task<WSPlataforma.DA.PEWSCryptoReference.SignerResponse> SignerAsync(string plainText, byte[] privateKey) {
            WSPlataforma.DA.PEWSCryptoReference.SignerRequest inValue = new WSPlataforma.DA.PEWSCryptoReference.SignerRequest();
            inValue.Body = new WSPlataforma.DA.PEWSCryptoReference.SignerRequestBody();
            inValue.Body.plainText = plainText;
            inValue.Body.privateKey = privateKey;
            return ((WSPlataforma.DA.PEWSCryptoReference.WSCryptoSoap)(this)).SignerAsync(inValue);
        }
    }
}
