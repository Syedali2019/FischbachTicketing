using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace FischbachTicketing.Common
{
    public class Mapper<T>
    {
        private readonly Func<IDataReader, T> mapFunction;

        public Mapper(Func<IDataReader, T> mapFunction)
        {
            this.mapFunction = mapFunction;
        }

        public T MapSingle(IDataReader reader)
        {
            return reader.Read() ? mapFunction(reader) : default(T);
        }

        public IEnumerable<T> MapList(IDataReader reader)
        {
            var list = new List<T>();

            using (reader)
            {
                while (reader.Read())
                {
                    var user = mapFunction(reader);
                    list.Add(user);
                }
            }

            return list;
        }
    }
}
