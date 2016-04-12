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
using System.Xml.Linq;
using System.Collections.Generic;
using ElectriCS.ThreePhase;

namespace ElectriCS
{
	public class Parser
	{
		public static ISolvable Parse (XDocument document)
		{
			switch (document.Root.Name.ToString()) {
			case	"kirchhoff":
			{
				return KirchhoffParse(document.Root);
				break;
			}
			case "radialnetwork":
			{
				IEnumerator<XElement> en = document.Root.Elements().GetEnumerator();
				en.MoveNext();
				return RadialNetworkParse(en.Current);
				break;
			}
			default:
				break;
			}
			return null;
		}
		private static KirchhoffCircuit KirchhoffParse (XElement element)
		{
			try {
				switch (element.Attribute("type").Value) {
				case "DC":
				{
					List<DC.Branch> branches = new List<ElectriCS.DC.Branch>();
					foreach (XElement el in element.Elements()){
						Dictionary<string,sbyte> rounds = new Dictionary<string, sbyte>();
						foreach(XElement rnds in el.Elements()){
							sbyte route;
							sbyte.TryParse(rnds.Attribute("route").Value, out route);
							rounds.Add(rnds.Attribute("name").Value, route);
						}
						double resistance	= Service.StrToDouble(el.Attribute("resistance").Value);
						double emf 				= Service.StrToDouble(el.Attribute("emf").Value);
						branches.Add(new ElectriCS.DC.Branch(el.Attribute("name").Value, el.Attribute("head").Value, el.Attribute("tail").Value, rounds, resistance, emf));
					}
					return new DC.KirchhoffCircuit(branches.ToArray());
				}
				case "AC":
				{
					List<AC.Branch> branches = new List<ElectriCS.AC.Branch>();
					foreach(XElement el in element.Elements()){
						Dictionary<string,sbyte> rounds = new Dictionary<string, sbyte>();
						foreach(XElement rnds in el.Elements()){
							sbyte route;
							sbyte.TryParse(rnds.Attribute("route").Value, out route);
							rounds.Add(rnds.Attribute("name").Value, route);
						}
						double resistance, reactance, module, phase, real, img;
						resistance		= Service.StrToDouble(el.Attribute("resistance").Value);
						reactance		= Service.StrToDouble(el.Attribute("reactance").Value);
						module			= Service.StrToDouble(el.Attribute("module").Value);
						phase				= Service.StrToDouble(el.Attribute("phase").Value);
						real 				= module * Math.Cos (phase);
						img 					= module * Math.Sin (phase);
						branches.Add(new ElectriCS.AC.Branch(el.Attribute("name").Value, el.Attribute("head").Value, el.Attribute("tail").Value, rounds, new Complex(resistance, reactance), new Complex(real, img)));
					}
					return new AC.KirchhoffCircuit(branches.ToArray());
				}
				default:
				{
					throw new InvalidXMLParse("KirchhoffParse");
				}
				}
			} catch {
				throw new InvalidXMLParse("KirchhoffParse");
			}
		}

		private static RadialNetwork RadialNetworkParse (XElement element)
		{
			string point = String.Empty;
			double voltage = 0;
			Element head;
			Dictionary<string,string> attr = new Dictionary<string, string> ();
			foreach (XAttribute a in element.Attributes()) {
				switch (a.Name.ToString ()) {
				case "volt":
					{
						voltage = Service.StrToDouble (a.Value);
						break;
					}
				case "point": 
					{
						point = a.Value;
						break;
					}
				default:
					{
						attr.Add (a.Name.ToString (), a.Value);
						break;
					}
				}
			}
			if (voltage == 0)
				throw new InvalidXMLParse ("Missing 'voltage'");
			head = ElementParse (element.Name.ToString (), attr);
			List<RadialNetwork> AuxTail = new List<RadialNetwork> ();
			foreach (XElement el in element.Elements()) {
				AuxTail.Add(RadialNetworkParse(el));
			}
			return new RadialNetwork(voltage, head, AuxTail.ToArray(), point);
		}

		private static Element ElementParse (string tip, Dictionary<string,string> args)
		{
			switch (tip) {
			case "element":
			{
				double R, X, R0, X0;
				R = (args.ContainsKey("resistance")) ? Service.StrToDouble(args["resistance"]) : 0;
				X = (args.ContainsKey("reactance")) ? Service.StrToDouble(args["reactance"]) : 0;
				R0 = (args.ContainsKey("zero_resistance")) ? Service.StrToDouble(args["zero_resistance"]) : 0;
				X0 = (args.ContainsKey("zero_reactance")) ? Service.StrToDouble(args["zero_reactance"]) : 0;
				return new Element(R, X, R0, X0);
			}
			case "system": return new ElectriCS.ThreePhase.System(args);// break;
			case "transformer": return new Transformer(args); //break;
			case "cable": return new Cable(args);// break;
			case "bus": return new Bus(args); //break;
			case "airway": return new Airway(args); 
			case "reactor": return new Reactor(args);
			default:
				throw new InvalidXMLParse("Invalid element name");
			}
		}
	}
}
