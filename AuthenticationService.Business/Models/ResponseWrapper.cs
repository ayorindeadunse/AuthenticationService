using System;
namespace AuthenticationService.Business.Models
{
	public class ResponseWrapper<T>
	{
		public bool IsRequestSuccessful { get; set; }
		public IEnumerable<string> Errors { get; set; }
		public T Data { get; set; }

		public ResponseWrapper()
		{
			IsRequestSuccessful = false;
		}
	}
}

