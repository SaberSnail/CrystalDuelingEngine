using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GoldenAnvil.Utility;
using GoldenAnvil.Utility.Logging;

namespace CrystalDuelingEngine.Tags
{
	public class TagCollection : IEnumerable<TagBase>
	{
		public TagCollection(IRenderable owner)
		{
			Owner = owner;
			m_tags = new List<TagBase>();
		}

		public TagCollection(IEnumerable<TagBase> tags, IRenderable owner)
		{
			Owner = owner;
			m_tags = tags.EmptyIfNull().ToList();
		}

		public IEnumerable<TagBase> Tags => m_tags;

		public IEnumerator<TagBase> GetEnumerator()
		{
			return Tags.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void AddTag(TagBase tag, KeyConflictResolutionKind conflictResolution)
		{
			Log.Info($"Adding tag to {Owner.RenderForLog()}: '{tag.RenderForLog()}'.");

			ConflictResolverBase conflictResolver = ConflictResolverBase.GetResolver(this, conflictResolution);
			conflictResolver.AddTag(tag);
		}

		public void RemoveTags(string key, MatchKind keyMatchKind)
		{
			var tagKeys = string.Join(", ", m_tags.Where(x => MatchKindUtility.IsMatch(x.Key, key, keyMatchKind, false)).Select(x => x.Key));
			Log.Info($"Removing tag from {Owner.RenderForLog()}: '{tagKeys}'.");

			m_tags.RemoveAll(x => MatchKindUtility.IsMatch(x.Key, key, keyMatchKind, false));
		}

		public void RemoveZeroDurationTags()
		{
			m_tags.RemoveAll(x => x.Duration.HasValue && x.Duration.Value <= 0);
		}

		public void DecrementDuration()
		{
			List<TagBase> newTags = new List<TagBase>();

			foreach (TagBase tag in m_tags)
			{
				if (tag.Duration.HasValue)
					newTags.Add(tag.CloneWithDuration(tag.Duration.Value - 1));
				else
					newTags.Add(tag);
			}

			m_tags.Clear();
			m_tags.AddRange(newTags);
		}

		private IRenderable Owner { get; }

		private abstract class ConflictResolverBase
		{
			public static ConflictResolverBase GetResolver(TagCollection tagCollection, KeyConflictResolutionKind conflictResolution)
			{
				switch (conflictResolution)
				{
				case KeyConflictResolutionKind.Add:
					return new AddConflictResolver(tagCollection.m_tags, tagCollection.Owner);
				case KeyConflictResolutionKind.Subtract:
					return new SubtractConflictResolver(tagCollection.m_tags, tagCollection.Owner);
				case KeyConflictResolutionKind.KeepAll:
					return new KeepAllConflictResolver(tagCollection.m_tags, tagCollection.Owner);
				case KeyConflictResolutionKind.KeepLarger:
					return new KeepLargerConflictResolver(tagCollection.m_tags, tagCollection.Owner);
				case KeyConflictResolutionKind.KeepSmaller:
					return new KeepSmallerConflictResolver(tagCollection.m_tags, tagCollection.Owner);
				case KeyConflictResolutionKind.Replace:
					return new ReplaceConflictResolver(tagCollection.m_tags, tagCollection.Owner);
				default:
					throw new ArgumentOutOfRangeException(nameof(conflictResolution), $"No conflict resolver available for resolution kind {conflictResolution}");
				}
			}

			public abstract void AddTag(TagBase tag);

			protected ConflictResolverBase(List<TagBase> tags, IRenderable owner)
			{
				Tags = tags;
				Owner = owner;
			}

			protected List<TagBase> Tags { get; }

			protected IRenderable Owner { get; }
		}

		sealed class KeepAllConflictResolver : ConflictResolverBase
		{
			public KeepAllConflictResolver(List<TagBase> tags, IRenderable owner)
				: base(tags, owner)
			{
			}

			public override void AddTag(TagBase tag)
			{
				Tags.Add(tag);
			}
		}

		private sealed class KeepLargerConflictResolver : ConflictResolverBase
		{
			public KeepLargerConflictResolver(List<TagBase> tags, IRenderable owner)
				: base(tags, owner)
			{
			}

			public override void AddTag(TagBase tag)
			{
				ValueTagBase valueTag = tag as ValueTagBase;
				if (valueTag == null)
					throw new InvalidRulesException(
						$"Tried to keep largest non-value tag '{tag.RenderForLog()}' on '{Owner.RenderForLog()}'.");

				TagBase tagToAdd = Tags
					.OfType<ValueTagBase>()
					.Where(x => x.Key == tag.Key)
					.Append(valueTag)
					.Max();

				Tags.RemoveAll(x => x.Key == tag.Key);
				Tags.Add(tagToAdd);
			}
		}

		private sealed class KeepSmallerConflictResolver : ConflictResolverBase
		{
			public KeepSmallerConflictResolver(List<TagBase> tags, IRenderable owner)
				: base(tags, owner)
			{
			}

			public override void AddTag(TagBase tag)
			{
				ValueTagBase valueTag = tag as ValueTagBase;
				if (valueTag == null)
					throw new InvalidRulesException(
						$"Tried to keep smallest non-value tag '{tag.RenderForLog()}' on '{Owner.RenderForLog()}'.");

				TagBase tagToAdd = Tags
					.OfType<ValueTagBase>()
					.Where(x => x.Key == tag.Key)
					.Append(valueTag)
					.Min();

				Tags.RemoveAll(x => x.Key == tag.Key);
				Tags.Add(tagToAdd);
			}
		}

		private sealed class ReplaceConflictResolver : ConflictResolverBase
		{
			public ReplaceConflictResolver(List<TagBase> tags, IRenderable owner)
				: base(tags, owner)
			{
			}

			public override void AddTag(TagBase tag)
			{
				Tags.RemoveAll(x => x.Key == tag.Key);
				Tags.Add(tag);
			}
		}

		private sealed class AddConflictResolver : ConflictResolverBase
		{
			public AddConflictResolver(List<TagBase> tags, IRenderable owner)
				: base(tags, owner)
			{
			}

			public override void AddTag(TagBase tag)
			{
				IAddableTag addableTag = tag as IAddableTag;
				if (addableTag == null)
					throw new InvalidRulesException($"Tried to sum non-addable tag '{tag.RenderForLog()}' on '{Owner.RenderForLog()}'.");

				foreach (IAddableTag that in Tags.Where(x => x.Key == tag.Key).OfType<IAddableTag>())
					addableTag = addableTag.AddTag(that);

				Tags.RemoveAll(x => x.Key == tag.Key);
				Tags.Add((TagBase) addableTag);
			}
		}

		private sealed class SubtractConflictResolver : ConflictResolverBase
		{
			public SubtractConflictResolver(List<TagBase> tags, IRenderable owner)
				: base(tags, owner)
			{
			}

			public override void AddTag(TagBase tag)
			{
				if (!(tag is ISubtractableTag))
					throw new InvalidRulesException($"Tried to sum non-subtractable tag '{tag.RenderForLog()}' on '{Owner.RenderForLog()}'.");

				ISubtractableTag newTag = null;
				foreach (ISubtractableTag that in Tags.Append(tag).Where(x => x.Key == tag.Key).OfType<ISubtractableTag>())
					newTag = newTag == null ? that : newTag.SubtractTag(that);

				Tags.RemoveAll(x => x.Key == tag.Key);
				Tags.Add((TagBase) newTag);
			}
		}

		private static readonly ILogSource Log = LogManager.CreateLogSource(nameof(TagCollection));

		readonly List<TagBase> m_tags;
	}
}
