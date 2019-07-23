using Blog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blog.Helpers
{
    public static class EnumHelper
    {
        public static List<SelectVm> PositionList()
        {
            var bodytype = Enum.GetValues(typeof(Position)).Cast<int>().ToList();
            var bodyname = Enum.GetNames(typeof(Position)).ToList();
            var body = new List<SelectVm>();
            for (int i = 0; i < bodytype.Count; i++)
            {
                body.Add(new SelectVm
                {
                    Id = bodytype[i],
                    Text = bodyname[i]
                });
            }
            return body;
        }
        public static List<SelectVm> StatusList()
        {
            var bodytype = Enum.GetValues(typeof(Status)).Cast<int>().ToList();
            var bodyname = Enum.GetNames(typeof(Status)).ToList();
            var body = new List<SelectVm>();
            for (int i = 0; i < bodytype.Count; i++)
            {
                body.Add(new SelectVm
                {
                    Id = bodytype[i],
                    Text = bodyname[i]
                });
            }
            return body;
        }
    }
}
