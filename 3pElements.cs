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
using System.Reflection;
using System.Numerics;
using System.Configuration;
using System.Collections.Generic;

namespace ElectriCS
{
	namespace ThreePhase
	{
        public enum Volume { Min, Max }
        public enum Mode { OnePhase, TwoPhase, ThreePhase }
        // TODO Реализовать с приведением напряжения к базовому
		public class Element : ElectriCS.IImpedance
		{
            double Voltage { get; set; }
			public double Resistance { get; set; }
			public double Reactance { get; set; }
			public double ZeroResistance { get; set; }
			public double ZeroReactance { get; set; }
			public Complex Impedance { get; set; }
			public Complex ZeroImpedance { get; set; }
			public double Abs { get; set; }
			public double ZeroAbs { get; set; }
			public Element (double resistance = 0, double reactance = 0, double zero_resistance =  0, double zero_reactance = 0)
			{
				Resistance = resistance;
				Reactance = reactance;
				ZeroReactance		= 2 * Resistance + zero_reactance;
				ZeroResistance		= 2 * Reactance + zero_resistance;
				Impedance				= new Complex(Resistance, Reactance);
				ZeroImpedance		= new Complex(ZeroResistance, ZeroReactance);
				Abs								= Impedance.Magnitude;
				ZeroAbs					= ZeroImpedance.Magnitude;
			}
            //TODO Протестить
            public static Element operator +(Element A, Element B)
            {
                Element Result          = new Element(A.Resistance + B.Resistance, A.Reactance + B.Reactance);
                Result.ZeroResistance   = A.ZeroResistance + B.ZeroResistance;
                Result.ZeroReactance    = A.ZeroReactance + B.ZeroReactance;
                Result.ZeroImpedance    = new Complex(Result.ZeroResistance, Result.ZeroReactance);
                return Result;
            }
			protected void Init (string methodName, Dictionary<string,string> args)
			{
				// TODO Использованы устаревшие методы чтения конфигурационных параметров
				// FIXME Реализовать обработку исключений.
                //string libName			= ConfigurationSettings.AppSettings["InitPath"] + ConfigurationSettings.AppSettings["InitName"];
                string libName = ConfigurationSettings.AppSettings["InitPath"] + ConfigurationSettings.AppSettings["InitName"] + ".dll";
                string typeName = ConfigurationSettings.AppSettings["InitName"] + ".Init";
				Assembly asm			= Assembly.LoadFrom(libName);
				Type init						= asm.GetType(typeName);
				object obj					= Activator.CreateInstance(init);
				MethodInfo met		= init.GetMethod(methodName);
				object[] arg				= new object[2] {this, args};
				met.Invoke(obj, arg);
			}

            public Element SetBaseVoltage(double BaseVoltage)
            {
                double Ratio = BaseVoltage / Voltage;
                return new Element(Ratio * Resistance, Ratio * Reactance, Ratio * ZeroResistance, Ratio * ZeroReactance);
            }
		}

		public class System : ElectriCS.ThreePhase.Element
		{
			public System (Dictionary<string,string> args)
			{
				Init("InitSystem", args);
			}
		}

		public class Cable : ElectriCS.ThreePhase.Element
		{
			public Cable (Dictionary<string,string> args)
			{
				Init("InitCable", args);
			}
		}

		public class Bus : ElectriCS.ThreePhase.Element
		{
			public Bus (Dictionary<string,string> args)
			{
				Init("InitBus", args);
			}
		}

		public class Transformer : ElectriCS.ThreePhase.Element
		{
			public Transformer (Dictionary<string,string> args)
			{
				Init("InitTransformer", args);
			}
		}

		public class Reactor : ElectriCS.ThreePhase.Element
		{
			public Reactor (Dictionary<string,string> args)
			{
				Init("InitReactor", args);	
			}
		}

		public class Airway : ElectriCS.ThreePhase.Element
		{
			public Airway (Dictionary<string,string> args)
			{
				Init("InitAirway", args);
			}
		}
        // DELETME Получше разобраться с реализацией приведения напряжения к базовому
		public class RadialNetwork : ISolvable
		{
			public string Point { get; set; }
			public Element Head { get; set; }
			public RadialNetwork[] Tail { get; set; }
            public double Voltage { get; set; }

			public RadialNetwork (double voltage, Element head, RadialNetwork[] tail = null, string point = "Unknown")
			{
				Voltage = voltage;
				Point 	= point;
				Head	= head;
				Tail		= tail;
			}

			private List<Element> Route (string point)
			{
				List<Element> Result = new List<Element>() { this.Head };
                if (point == this.Point)
                {
                    return Result;
                }
                foreach (RadialNetwork element in this.Tail)
                {
                    List<Element> aux = element.Route(point);
                    if (!(aux == null))
                    {
                        Result.AddRange(aux);
                        return Result;
                    }
                }
                return null;
			}

            // Расчёты
            private Complex GetRouteImpedance(Element Result, Mode mode)
            {
                switch (mode)
                {
                    case Mode.OnePhase:
                        return Result.ZeroImpedance / 3;
                    case Mode.TwoPhase:
                        return (2 / Math.Sqrt(3)) * Result.Impedance;
                    case Mode.ThreePhase:
                        return Result.Impedance;
                    default:
                        return 0;
                }
            }

            public double SCCSolve (string point, Mode mode = Mode.ThreePhase, Volume volume = Volume.Max)
			{
				List<Element> route = Route (point);
				if (route == null)
					throw new InvalidRadialCircuit ("Route error");
                route.ForEach(el => el.SetBaseVoltage(Voltage));
				Element RouteElem = new Element ();
				foreach (Element el in route) {
					RouteElem = RouteElem + el;
				}
				Complex RouteImp = GetRouteImpedance (RouteElem, mode);
				if (RouteImp == 0) {
					throw new InvalidRadialCircuit ("Routing error"); //TODO Уточнить сообщение
				}
				if (Voltage < 1000) {
					double i0 = Voltage / (Math.Sqrt (3) * RouteImp.Magnitude);
					switch (volume) {
					case Volume.Max: 
						{
							return i0;
						}
					case Volume.Min:
						{
							double aZ = RouteImp.Real;
							double Ks = 0.6 - 2.5 * aZ + 0.114 * Math.Sqrt (1000 * aZ) - 0.13 * Math.Pow (1000 * aZ, (1 / 3.0));
							Element Arc = new Element ((Math.Sqrt ((Math.Pow (Voltage / (i0 * Ks), 2) / 3) - Math.Pow (RouteElem.Reactance, 2)) - RouteElem.Resistance));
							RouteElem = RouteElem + Arc;
							RouteImp = GetRouteImpedance (RouteElem, mode);
							return Voltage / (Math.Sqrt (3) * RouteImp.Magnitude);
						}
					default:
						return 0; //TODO Уточнить
					}
				} else {
					double c = (volume == Volume.Max) ? 1.1 : 1.0;
					return c * Voltage / (Math.Sqrt(3) * RouteImp.Imaginary);
				}
            }

            public double VoltageLossSolve(string point, Mode mode, Dictionary<string, string> args)
            {
                List<Element> route = Route (point);
				if (route == null)
					throw new InvalidRadialCircuit ("Route error");
                Element RouteResult = new Element();
                foreach(Element el in route) RouteResult += el;
                double cos = 1, Coeff;
                if(args.ContainsKey("cos"))
                {
                    cos = ElectriCS.Service.StrToDouble(args["cos"]);
                }
                double Voltage;
                if(args.ContainsKey("voltage"))
                {
                    Voltage = ElectriCS.Service.StrToDouble(args["voltage"]);
                }
                else
                {
                    throw new InvalidArgument();
                }
                if (args.ContainsKey("amperage"))
                {
                    double Amperage = ElectriCS.Service.StrToDouble(args["amperage"]);
                    switch (mode)
                    {
                        case Mode.OnePhase:
                            {
                                Coeff = 200 * Math.Sqrt(3) * Amperage / Voltage;
                                break;
                            }
                        case Mode.ThreePhase:
                            {
                                Coeff = 100 * Math.Sqrt(3) * Amperage / Voltage;
                                break;
                            }
                        default:
                            {
                                throw new InvalidArgument();
                                //break;
                            }
                    }
                    return Coeff * (cos * RouteResult.Resistance + RouteResult.Reactance * Math.Sin(Math.Acos(cos)));
                }
                if(args.ContainsKey("power"))
                {
                    double P = ElectriCS.Service.StrToDouble(args["power"]);
                    double Q = P * Math.Tan(Math.Acos(cos));
                    switch (mode)
                    {
                        case Mode.OnePhase:
                            {
                                Coeff = 600 / Math.Pow(Voltage, 2);
                                break;
                            }
                        case Mode.ThreePhase:
                            {
                                Coeff = 100 / Math.Pow(Voltage, 2);
                                break;
                            }
                        default:
                            {
                                throw new InvalidArgument();
                                break;
                            }
                    }
                    return Coeff * (RouteResult.Resistance * P + RouteResult.Reactance * Q);
                }
                throw new InvalidRadialCircuit();
            }
		}
	}
}