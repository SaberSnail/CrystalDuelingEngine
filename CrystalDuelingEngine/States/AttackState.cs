using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CrystalDuelingEngine.Tags;
using GoldenAnvil.Utility;

namespace CrystalDuelingEngine.States
{
	public sealed class AttackState
	{
		public AttackState()
		{
			m_tags = new Dictionary<TagScope, IEnumerable<TagBase>>();
		}

		public void SetTags(TagScope scope, IEnumerable<TagBase> tags)
		{
			m_tags[scope] = tags.EmptyIfNull().ToList();
		}

		public void SetDynamicTags(TagScope scope, TagCollection tags)
		{
			m_tags[scope] = tags;
		}

		public ReadOnlyCollection<TagBase> GetTags(TagScope scope)
		{
			return Enum.GetValues(typeof(TagScope))
				.Cast<TagScope>()
				.Where(x => scope.HasFlag(x))
				.Select(x => m_tags.GetValueOrDefault(x))
				.WhereNotNull()
				.SelectMany(x => x)
				.ToList()
				.AsReadOnly();
		}

		readonly Dictionary<TagScope, IEnumerable<TagBase>> m_tags;
	}
}
