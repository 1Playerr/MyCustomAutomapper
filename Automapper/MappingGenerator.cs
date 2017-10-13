using System;
using System.Collections.Generic;

namespace MyCustomAutomapper
{
	public class MappingGenerator
	{
		private Dictionary<string, object> _mappings;

		public MappingGenerator()
		{
			_mappings = new Dictionary<string, object>();
		}

		public Mapper<TSource, TDestination> Create<TSource, TDestination>()
		{
			string key = this.GetKey<TSource, TDestination>();

			if (!_mappings.ContainsKey(key))
			{
				Mapper<TSource, TDestination> mapper = ExpressionHelper.Generate<TSource, TDestination>();
				_mappings[key] = mapper;

				return mapper;
			}

			throw new Exception("Something wrong.");
		}

		private string GetKey<TSource, TDestination>()
		{
			return $"{typeof(TSource).Name}-{typeof(TDestination).Name}";
		}
	}
}
