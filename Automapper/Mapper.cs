using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace MyCustomAutomapper
{
	public class Mapper<TSource, TDestination>
	{
		internal Expression<Func<TSource, TDestination>> MapExpression { get; set; }
		internal HashSet<MemberAssignment> Assignments { get; set; }

		internal Mapper(Expression<Func<TSource, TDestination>> exp, HashSet<MemberAssignment> assignments)
		{
			MapExpression = exp;
			Assignments = assignments;
		}

		public TDestination Map(TSource source)
		{
			return MapExpression.Compile()(source);
		}
	}
}