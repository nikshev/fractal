using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Optimera;

namespace Fractal
{
    class Model: IOptimisable
    {
        private double old_price;//clo->stock_price; 
        private double old_volatilities=0;//clo->variance;
        private double mean_reversion_rate=0;
        private double mean_reversion_level=0;
        private double dt=0;
        private double variance_variance=0;
        private double risk_free_rate=0;
        private double H;
        private List<double> output;
        private List<double> output2;
        private double NeededPrice;
        private double mt_MAPrice;
        private int Count;

        public int NumberOfParameters()
        {
            return 5;
        }

        public object DeepClone()
        {
            Model clone = new Model(old_price, H, output, output2, mt_MAPrice, Count, old_volatilities);
            clone.old_volatilities = this.old_volatilities;
            clone.mean_reversion_rate = this.mean_reversion_rate;
            clone.mean_reversion_level = this.mean_reversion_level;
            clone.dt = this.dt;
            clone.variance_variance = this.variance_variance;
            clone.risk_free_rate = this.risk_free_rate;
            return clone;
        }

        public Model(double OPrice, double HC, List<double> o, List<double> o2, double MAPrice, int count, double volatilities)
        {
            old_price = OPrice;
            H = HC;
            output=new List<double>();
            output2 = new List<double>();
            output = o;
            output2 = o2;
            mt_MAPrice = MAPrice;
            Count = count;
            old_volatilities = volatilities;
        }

        public double Fitness(double[] genes)
        {
            SetParams(genes[0], genes[1], genes[2], genes[3], genes[4]);
            double temp=Calculate();
            double retVal=0;
            if (Double.IsNaN(temp))
                retVal = -10000000;
            else
                retVal=1/temp;
            return retVal;
        }

        public void SetParams(Double a, Double b, Double c, Double d, Double e)
        {
           // old_volatilities = mt_MAPrice;
            mean_reversion_rate = a/10000000;
            mean_reversion_level = b/10000000;
            dt = c/10000000;
            variance_variance = d/10000000;
            risk_free_rate = e/10000000;
        }

        public double Calculate()
        {
            double old_price_loc = old_price;
            double old_volatilities_loc = old_volatilities;
            double MA = 0;
            for (int i = 0; i < Count; i++)
            {
                 double new_volatilities = old_volatilities_loc +
                               mean_reversion_rate * (mean_reversion_level - old_volatilities_loc) * dt +
                               variance_variance * output[i];

                  double new_price = old_price_loc +
                              risk_free_rate * old_price_loc * dt +
                              new_volatilities * old_price_loc * output2[i]; //These are already scaled via hoskin */
                  if (new_price > 0)
                      MA += Math.Abs(new_price - mt_MAPrice) * Math.Abs(new_price - mt_MAPrice);
                  else
                      MA += 10000000;    
                old_price_loc = new_price;
                old_volatilities_loc = new_volatilities;
            }
            return (MA/Count);
          //  return (Error);
        }
    }
}
