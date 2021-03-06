﻿using System.Collections.Generic;

namespace TM.Discovery.V2.Models
{
    public class SearchVenuesRequest : BaseQuery<SearchVenuesQueryParameters>
    {
        public override void AddQueryParameter(KeyValuePair<SearchVenuesQueryParameters, string> parameter)
        {
            ParametersDictionary.Add(parameter.Key.ToString(), parameter.Value);
        }
    }
}
