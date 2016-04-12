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
using System.Collections.Generic;
using System.Numerics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace ElectriCS
{
	namespace AC
	{
		public class Branch:ElectriCS.Branch
		{
			public Complex Impedance {
				get;
				set;
			}
			public Complex EMF{
				get;
				set;
			}
			public Branch (string name, string head, string tail, Dictionary<string,sbyte> rounds, Complex impedance, Complex emf):base(name,head,tail,rounds)
			{
				EMF 				= emf;
				Impedance	= impedance;
			}
		}
		public class KirchhoffCircuit:ElectriCS.KirchhoffCircuit
		{
			public KirchhoffCircuit (params AC.Branch[] arg):base(arg)
			{
			}
			public Dictionary<string, Complex> Solve ()
			{
				Complex [ , ] Matr = new Complex[Names.Length, Names.Length];
				Complex [] Vect = new Complex[Names.Length];
				int LawOne = Nodes.Length - 1;
				int LawTwo = Names.Length - LawOne;
				if ((LawTwo > Rounds.Length) || (!(LawOne > 0)) || (!(LawTwo > 0)))
					throw new InvalidCircuit ("in KirchhoffCircuit");
				for (int i = 0; i < LawOne; i++) {
					for (int j = 0; j < Circuit.Length; j++) {
						AC.Branch Aux = (AC.Branch)Circuit [j];
						Matr [i, j] = Aux.GetNode (Nodes [i]);
					}
					Vect [i] = new Complex ();
				}
				for (int i = LawOne; i < Names.Length; i++) {
					Complex e = new Complex ();
					for (int j = 0; j < Circuit.Length; j++) {
						AC.Branch Aux = (AC.Branch)Circuit [j];
						Matr [i, j] = Aux.GetRound (Rounds [i - LawOne]) * Aux.Impedance;
						e = e + Aux.GetRound (Rounds [i - LawOne]) * Aux.EMF;
					}
					Vect [i] = e;
				}
				var Matrik = Matrix<Complex>.Build.DenseOfArray (Matr);
				var Vector = Vector<Complex>.Build.Dense (Vect);
				var Result = Matrik.Solve (Vector);
				Dictionary<string, Complex> Resultat = new Dictionary<string, Complex> ();
				Complex [] Comp = Result.ToArray ();
				for (int i = 0; i < Names.Length; i++) {
					Resultat.Add (Names [i], Comp [i]);
				}
				return Resultat;
			}
		}
	}
}