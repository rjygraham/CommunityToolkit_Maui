﻿using CommunityToolkit.Maui.Behaviors;
using Xunit;

namespace CommunityToolkit.Maui.UnitTests.Behaviors;

public class IconTintColorBehavior_Tests : BaseTest
{
	[Fact]
	public void VerifyDefaultColor()
	{
		var iconTintColorBehavior = new IconTintColorBehavior();

		Assert.Equal(default, iconTintColorBehavior.TintColor);
		Assert.Equal(null, iconTintColorBehavior.TintColor);
	}

	[Fact]
	public void VerifyColorChanged()
	{
		var iconTintColorBehavior = new IconTintColorBehavior();

		Assert.Equal(default, iconTintColorBehavior.TintColor);

		iconTintColorBehavior.TintColor = Colors.Blue;

		Assert.Equal(Colors.Blue, iconTintColorBehavior.TintColor);
	}
}
