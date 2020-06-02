using FlightControlWeb.Model;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlightControlWeb.Models
{
    public class FlightPlansManager
    {

        public void idCreate(FlightPlan fp)
        {
            string time = fp.data_time.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            string time1 = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fff");
            string company = fp.company_name;
            string newId = "";
            int i;
            for (i = 0; i < 6; i++)
            {
                if ((time1[i + 5] != '-') && (time1[i + 5] != ':')) {
                    newId += time1[i + 5];
                }
                else
                {
                    newId += time1[i + 6];
                }
                newId += company[i % company.Length];
            }
            fp.Id = newId;
        }

        public void createSegments(FlightPlan fp , string segments_input)
        {
            int index, i1, i2, i3, first, second;
            int index2, index3;
            index = segments_input.IndexOf("segments");
            string cut_input = segments_input.Substring(index, segments_input.Length - index);
            string longitude, latitude, timespan_seconds;
            string longitu = "longitude: ", latitu = "latitude: ", timespan = "timespan_seconds: ";
            index = 0;
            while (index < cut_input.IndexOf("]") - 3)
            {
                index2 = cut_input.IndexOf("{", index);
                index3 = cut_input.IndexOf("}", index);
                string cut_segment = cut_input.Substring(index2, index3 - index2 + 1);
                i1 = cut_segment.IndexOf("longitude");
                i2 = cut_segment.IndexOf("latitude");
                i3 = cut_segment.IndexOf("timespan_seconds");
                if (i1 == -1 || i2 == -1 || i3 == -1)
                {
                }
                i1 += longitu.Length + 1;
                i2 += latitu.Length + 1;
                i3 += timespan.Length + 1;
                first = cut_segment.IndexOf(",");
                second = cut_segment.IndexOf(",", first + 1);
                longitude = cut_segment.Substring(i1, first - i1);
                latitude = cut_segment.Substring(i2, second - i2);
                timespan_seconds = cut_segment.Substring(i3, cut_segment.IndexOf("}") - i3 - 2);
                Segment s = new Segment
                {
                    longitude = Convert.ToDouble(longitude),
                    latitude = Convert.ToDouble(latitude),
                    timespan_seconds = Convert.ToInt32(timespan_seconds)
                };
                //s.Id = fp.Id + fp.segments.Count;
                //DB.segments.AddAsync(s);
                fp.segments.Add(s);
                index = index3 + 1;
            }
        }


    }
}
