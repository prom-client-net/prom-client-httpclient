//using System;
//using System.Linq;
//using System.Net.Http;
//using System.Text;
//using System.Threading.Tasks;
//using System.Threading;
//using System.Net;

//namespace Prometheus.Client.HttpClient.Tests
//{
//    internal class TestDelegatingHandler : DelegatingHandler
//    {
//        private readonly Func<HttpResponseMessage> _actionToExecute;
//        public TestDelegatingHandler(Func<Union> actionToExecute)
//            :base()
//        {
//            _actionToExecute = actionToExecute;
//        }

//        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
//        {
//            return Task.FromResult<HttpResponseMessage>(new HttpResponseMessage());
//        }
//    }
//}
