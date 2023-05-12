using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HackerNews.Utils;

namespace HackerNews;

partial class NewsViewModel : BaseViewModel
{
	readonly TextAnalysisService _textAnalysisService;
	readonly HackerNewsAPIService _hackerNewsAPIService;


	[ObservableProperty]
	bool _isListRefreshing;

	public NewsViewModel(TextAnalysisService textAnalysisService,
						 HackerNewsAPIService hackerNewsAPIService)
	{
		_textAnalysisService = textAnalysisService;
		_hackerNewsAPIService = hackerNewsAPIService;
	}

	public event EventHandler<string>? PullToRefreshFailed;


	[ObservableProperty]
	List<StoryModel> topStoryCollection = new();

	[RelayCommand]
	async Task Refresh()
	{
		var hash = new HashSet<StoryModel>();
		try
		{
			await foreach (var story in GetTopStories(StoriesConstants.NumberOfStories).ConfigureAwait(false))
			{
				StoryModel? updatedStory = null;
				
				try
				{
					updatedStory = story with { TitleSentiment = await _textAnalysisService.GetSentiment(story.Title)};
					hash.Add(updatedStory);
				}
				catch (Exception)
				{
					updatedStory = story;
				}
			}
		}
		catch (Exception e)
		{
			OnPullToRefreshFailed(e.ToString());
		}
		finally
		{
			ThreadHelpers.BeginInvokeOnMainThread(() => TopStoryCollection = new (hash));
			IsListRefreshing = false;
		}
	}

	async IAsyncEnumerable<StoryModel> GetTopStories(int? storyCount = int.MaxValue)
	{
		var topStoryIds = await _hackerNewsAPIService.GetTopStoryIDs().ConfigureAwait(false);
		var getTopStoryTaskList = topStoryIds.Select(_hackerNewsAPIService.GetStory).ToList();

		while (getTopStoryTaskList.Any() && storyCount-- > 0)
		{
			var completedGetStoryTask = await Task.WhenAny(getTopStoryTaskList).ConfigureAwait(false);
			getTopStoryTaskList.Remove(completedGetStoryTask);

			var story = await completedGetStoryTask.ConfigureAwait(false);
			yield return story;
		}
	}

	void OnPullToRefreshFailed(string message) => PullToRefreshFailed?.Invoke(this, message);
}