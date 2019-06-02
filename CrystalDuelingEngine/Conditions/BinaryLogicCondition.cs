using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrystalDuelingEngine.Serialization;
using CrystalDuelingEngine.States;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.Conditions
{
	public abstract class BinaryLogicCondition : ConditionBase
	{
		public ReadOnlyCollection<ConditionBase> Children { get; }

		public override bool IsTrue(AttackState state)
		{
			bool? result = Children.EmptyIfNull().Aggregate<ConditionBase, bool?>(null, (current, child) => CombineResult(current, child.IsTrue(state)));
			return result.GetValueOrDefault();
		}

		public override string RenderForLog()
		{
			return $"({string.Join($" {RenderOperatorForLog()} ", Children.Select(x => x.RenderForLog()))})";
		}

		public override string RenderForUi()
		{
			return $"({string.Join($" {RenderOperatorForUi()} ", Children.Select(x => x.RenderForLog()))})";
		}

		public override void Serialize(ISerializer serializer)
		{
			serializer.SerializeValue(nameof(Children), Children);
			base.Serialize(serializer);
		}

		protected BinaryLogicCondition(IEnumerable<ConditionBase> children)
		{
			Children = children.EmptyIfNull().ToList().AsReadOnly();
		}

		protected BinaryLogicCondition(IDeserializer deserializer)
			: base(deserializer)
		{
			Children = deserializer.GetValues<ConditionBase>(nameof(Children)).EmptyIfNull().ToList().AsReadOnly();
		}

		protected abstract bool CombineResult(bool? current, bool result);

		protected abstract string RenderOperatorForLog();

		protected abstract string RenderOperatorForUi();

		protected override bool IsValidCore(List<string> errors)
		{
			if (Children.Count < 2)
			{
				errors.Add(OurResources.InvalidConditionMissingChildren.FormatCurrentCulture(GetType().Name, 2));
				return false;
			}

			return Children.All(x => x.IsValid(errors));
		}
	}
}
