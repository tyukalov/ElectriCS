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
using System.Numerics;
using System.Collections.Generic;


namespace ElectriCS
{
	//  ******** Primary interfaces ********
//	public interface IResistance
//	{
//		double Resistance { get; set; }
//	}
//	public interface IReactance
//	{
//		double Reactance { get; set; }
//	}
//	public interface IInductance
//	{
//		double Inductance { get; set; }
//	}
//	public interface ICapacitance
//	{
//		double Capacitance { get; set; }
//	}
	public interface ISolvable
	{

	}
	public interface IBranch
	{
		string Name { get; set; }
		string Head { get; set; }
		string Tail { get; set; }
		Dictionary<string,sbyte> Rounds { get; set; }
//		sbyte GetNode(string node);
//		sbyte GetRound(string round);
	}

	public interface IImpedance
	{
		double Resistance { get; set; }
		double Reactance { get; set; }
		double ZeroResistance { get; set; }
		double ZeroReactance { get; set; }
		Complex Impedance { get; set; }
		Complex ZeroImpedance { get; set; }
	}

	//  ******** Derivative interfaces ********
//	public interface IImpedance : IResistance, IReactance
//	{
//		Complex Impedance { get;  }
//		double Module { get; set; }
//	}
}

