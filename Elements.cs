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
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace ElectriCS
{
	public abstract class Branch:IBranch 
	{
		public string Name { get; set; }
		public string Head { get; set; }
		public string Tail { get; set; }
		public Dictionary<string, sbyte> Rounds { get; set; }
		public Branch (string name, string head, string tail, Dictionary<string, sbyte> rounds)
		{
			Name 						= name;
			Head 						= head;
			Tail 							= tail;
			Rounds					= rounds;
		}
		public sbyte GetNode (string node)
		{
			if (node == Head) return 1;
			if (node == Tail) return -1;
			return 0;
		}
		public sbyte GetRound (string round)
		{
			if (Rounds.ContainsKey(round)) return Rounds[round];
			else return 0;
		}
	}
	public abstract class KirchhoffCircuit:ISolvable
	{
		protected string[] Names;
		protected string[] Nodes;
		protected string[] Rounds;
		public IBranch [] Circuit { get; set; }
		public KirchhoffCircuit (params IBranch[] arg)
		{
			List<string> lstNames = new List<string> ();
			List<string> lstNodes = new List<string> ();
			List<string> lstRounds = new List<string> ();
			Circuit = (IBranch[])arg.Clone ();
			foreach (IBranch branch in Circuit) {
				if (!(lstNames.Contains (branch.Name)))
					lstNames.Add (branch.Name);
				if (!(lstNodes.Contains (branch.Head)))
					lstNodes.Add (branch.Head);
				if (!(lstNodes.Contains (branch.Tail)))
					lstNodes.Add (branch.Tail);
				foreach (string rnd in branch.Rounds.Keys) {
					if (!(lstRounds.Contains (rnd)))
						lstRounds.Add (rnd);
				}
			}
			Names 	= lstNames.ToArray();
			Nodes 	= lstNodes.ToArray();
			Rounds	= lstRounds.ToArray();
		}
	}
}
