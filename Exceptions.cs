//
//  Author:
//    Igor Tyukalov tyukalov@bk.ru
//
//  Copyright (c) 2016, sp_r00t
//
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//     * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//     * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//       the documentation and/or other materials provided with the distribution.
//     * Neither the name of the [ORGANIZATION] nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
//

using System;

namespace ElectriCS 
{
	public class InvalidArgument : ApplicationException
	{
		private string localMessage = String.Empty;
		public InvalidArgument(){}
		public InvalidArgument (string msg)
		{
			localMessage = msg;
		}
		public override string Message {
			get {
				return string.Format("Invalid argument:  {0}", localMessage);
			}
		}
	}
	public class InvalidCircuit : ApplicationException
	{
		private string localMessage = String.Empty;
		public InvalidCircuit(){}
		public InvalidCircuit(string msg)
		{
			localMessage = msg;
		}
		public override string Message {
			get {
				return string.Format("Invalid circuit:  {0}", localMessage);
			}
		}
	}
	public class InvalidXMLParse : ApplicationException
	{
		private string localMessage = String.Empty;
		public InvalidXMLParse(){}
		public InvalidXMLParse (string msg)
		{
			localMessage = msg;
		}
		public override string Message {
			get {
				return string.Format("XML parse error in:  {0}", localMessage);
			}
		}
	}

	public class InvalidRadialCircuit : ApplicationException
	{
		private string localMessage = String.Empty;
		public InvalidRadialCircuit(){}
		public InvalidRadialCircuit (string msg)
		{
			localMessage = msg;
		}
		public override string Message {
			get {
				return string.Format("XML parse error in:  {0}", localMessage);
			}
		}
	}
}