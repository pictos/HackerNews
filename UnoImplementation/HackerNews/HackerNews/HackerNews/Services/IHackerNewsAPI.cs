﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Refit;

namespace HackerNews;

public interface IHackerNewsAPI
{
	[Get("/topstories.json?print=pretty")]
	Task<IReadOnlyList<long>> GetTopStoryIDs();

	[Get("/item/{storyId}.json?print=pretty")]
	Task<StoryModel> GetStory(long storyId);
}