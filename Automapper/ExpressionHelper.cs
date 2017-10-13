using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MyCustomAutomapper
{
	public static class ExpressionHelper
	{
		private static HashSet<MemberAssignment> _assignments = new HashSet<MemberAssignment>();
		private static ParameterExpression _sourceParam = null;

		public static Mapper<TSource, TDestination> ForMember<TSource, TDestination>(
			this Mapper<TSource, TDestination> mapper,
			Expression<Func<TSource, object>> from,
			Expression<Func<TDestination, object>> to)
		{
			if (from == null || to == null)
			{
				return mapper;
			}

			return mapper.Regenerate(from, to);
		}

		internal static Mapper<TSource, TDestination> Generate<TSource, TDestination>()
		{
			ParameterExpression sourceParam = Expression.Parameter(typeof(TSource));
			_sourceParam = sourceParam;
			var body = GetBody<TSource, TDestination>(sourceParam);
			var mapFunction = Expression.Lambda<Func<TSource, TDestination>>(body, sourceParam);

			return new Mapper<TSource, TDestination>(mapFunction, _assignments);
		}

		private static Mapper<TSource, TDestination> Regenerate<TSource, TDestination>(
			this Mapper<TSource, TDestination> mapper,
			Expression<Func<TSource, object>> from,
			Expression<Func<TDestination, object>> to)
		{
			var fromME = from.GetMemberExpression();
			var toME = to.GetMemberExpression();

			bool isCustomMemberAssignmentInList = mapper.Assignments.Select(x => x.Member.Name).Any(x => x == fromME.Member.Name);
			if (isCustomMemberAssignmentInList)
			{
				return mapper;
			}

			Type destType = typeof(TDestination);
			var assignment = Expression.Bind(
						destType.GetProperty(toME.Member.Name),
						Expression.Property(_sourceParam, fromME.Member.Name));
			mapper.Assignments.Add(assignment);

			var body = Expression.MemberInit(Expression.New(destType), mapper.Assignments);
			mapper.MapExpression = Expression.Lambda<Func<TSource, TDestination>>(body, _sourceParam);

			return mapper;
		}

		private static MemberInitExpression GetBody<TSource, TDestination>(ParameterExpression sourceParam)
		{
			Type destType = typeof(TDestination);
			var srcProps = sourceParam.Type.GetProperties();

			HashSet<MemberAssignment> assignments = new HashSet<MemberAssignment>();

			foreach (var prop in srcProps)
			{
				var propInfo = destType.GetProperty(prop.Name);
				if (propInfo == null)
				{
					continue;
				}
				var assignment = Expression.Bind(
						propInfo,
						Expression.Property(sourceParam, prop));
				assignments.Add(assignment);
			}
			_assignments = assignments;

			return Expression.MemberInit(Expression.New(destType), assignments);
		}

		private static MemberExpression GetMemberExpression<T>(this Expression<Func<T, object>> exp)
		{
			MemberExpression result = null;
			var unaryExperession = exp.Body as UnaryExpression;
			var memberExpressionFromOperand = unaryExperession?.Operand as MemberExpression;
			var memberExpressionFromBody = exp.Body as MemberExpression;
			result = memberExpressionFromOperand ?? memberExpressionFromBody;

			if (result == null)
			{
				throw new Exception();
			}

			return result;
		}
	}
}
