﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Commpoint.Utility.SmsService {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SmsService.ISmsService")]
    public interface ISmsService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ISmsService/SendSms", ReplyAction="http://tempuri.org/ISmsService/SendSmsResponse")]
        string SendSms(string apiKey, string accessKey, string body, string user);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ISmsServiceChannel : Commpoint.Utility.SmsService.ISmsService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class SmsServiceClient : System.ServiceModel.ClientBase<Commpoint.Utility.SmsService.ISmsService>, Commpoint.Utility.SmsService.ISmsService {
        
        public SmsServiceClient() {
        }
        
        public SmsServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public SmsServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SmsServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public SmsServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string SendSms(string apiKey, string accessKey, string body, string user) {
            return base.Channel.SendSms(apiKey, accessKey, body, user);
        }
    }
}
