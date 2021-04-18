<?xml version="1.0" encoding="utf-8"?>
<Grid
  xmlns="www.team-mediaportal.com/2008/mpf/directx"
  xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
  xmlns:fanart="clr-namespace:MediaPortal.Extensions.UserServices.FanArtService.Client;assembly=FanArtService.Client"
  xmlns:media="clr-namespace:MediaPortal.UiComponents.Media.Models.AspectWrappers;assembly=Media"
  xmlns:emulators="clr-namespace:Emulators.MediaExtensions;assembly=Emulators"
  x:Name="MediaItemDetails" HorizontalAlignment="Stretch" VerticalAlignment="Stretch"
  DataContext="{Binding Source={StaticResource FanArtBackgroundModel},Path=SelectedItem}">

  <Grid.Resources>
    <fanart:FanArtImageSourceConverter x:Key="FanArtImageSourceConverter"/>
  </Grid.Resources>
  <Grid.RowDefinitions>
    <RowDefinition Height="Auto"/>
    <RowDefinition Height="Auto"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="Auto"/>
  </Grid.ColumnDefinitions>
  
  <emulators:GameAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" Margin="0,5,20,-5" >
    <emulators:GameAspectWrapper.Template>
      <ControlTemplate>
         <Grid DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
               IsVisible="{Binding ElementName=GameName, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
          <Label x:Name="GameName" DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                 Content="{Binding GameName}" FontSize="{ThemeResource SmallerFontSize}" Wrap="True" TextTrimming="WordEllipsis"
                 Color="{ThemeResource TextColor}" HorizontalAlignment="Stretch" />
        </Grid>
      </ControlTemplate>
    </emulators:GameAspectWrapper.Template>
  </emulators:GameAspectWrapper>


  <emulators:GameAspectWrapper Grid.Row="1" MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}">
    <emulators:GameAspectWrapper.Template>
      <ControlTemplate>
        <StackPanel Orientation="Horizontal" Margin="0,0,10,0" DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">
          <Grid>
            <Grid.RowDefinitions>
              <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <Image x:Name="YearIcon" Source="HomeContentIcons\year.png" Stretch="Uniform" Height="25" VerticalAlignment="Center" Margin="0,-1,0,1"
                   IsVisible="{Binding Path=Year,Converter={StaticResource ReferenceToBoolConverter}}"/>
            <Label x:Name="Year" Grid.Column="1" FontSize="{ThemeResource SmallerFontSize}" Color="{ThemeResource TextColor}" VerticalAlignment="Center"
                   IsVisible="{Binding Path=Year,Converter={StaticResource ReferenceToBoolConverter}}"
                   Content="{Binding Year}" Margin="5,0,30,0" Opacity="0.6"/>
          </Grid>

          <Grid>
            <Grid.RowDefinitions>
              <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <Image x:Name="RatingIcon" Source="HomeContentIcons\Star.png" Stretch="Uniform" Height="25" VerticalAlignment="Center" Margin="0,-1,0,1"
                   IsVisible="{Binding ElementName=RatingCountLabel, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}"/>
            <Label x:Name="RatingCountLabel" Margin="5,0,30,0" VerticalAlignment="Center" Grid.Column="1"
                   FontSize="{ThemeResource SmallerFontSize}" Color="{ThemeResource TextColor}" Opacity="0.6"
                   IsVisible="{Binding Path=Rating,Converter={StaticResource ReferenceToBoolConverter}}">
              <Label.Content>
                <Binding Path="Rating" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}{0:F1}"/>
              </Label.Content>
            </Label>
          </Grid>

          <Grid>
            <Grid.RowDefinitions>
              <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <Image x:Name="PlatformIcon" Source="HomeContentIcons\Game.png" Stretch="Uniform" Height="25" VerticalAlignment="Center" Margin="0,-2,0,2"
                   IsVisible="{Binding ElementName=PlatformLabel, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}"/>
            <Label x:Name="PlatformLabel" Margin="5,0,30,0" VerticalAlignment="Center" Grid.Column="1" Content="{Binding Platform}"
                   FontSize="{ThemeResource SmallerFontSize}" Color="{ThemeResource TextColor}" Opacity="0.6"
                   IsVisible="{Binding Path=Platform,Converter={StaticResource ReferenceToBoolConverter}}">
            </Label>
          </Grid>

          <Grid>
            <Grid.RowDefinitions>
              <RowDefinition Height="*"/>
            </Grid.RowDefinitions>
            <Grid.ColumnDefinitions>
              <ColumnDefinition Width="Auto"/>
              <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <Image x:Name="DeveloperIcon" Source="HomeContentIcons\Brand.png" Stretch="Uniform" Height="25" VerticalAlignment="Center" Margin="0,-1,0,1"
                   IsVisible="{Binding ElementName=PlatformLabel, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}"/>
            <Label x:Name="DeveloperLabel" Margin="5,0,30,0" VerticalAlignment="Center" Grid.Column="1" Content="{Binding Developer}"
                   FontSize="{ThemeResource SmallerFontSize}" Color="{ThemeResource TextColor}" Opacity="0.6"
                   IsVisible="{Binding Path=Developer,Converter={StaticResource ReferenceToBoolConverter}}">
            </Label>
          </Grid>

        </StackPanel>
      </ControlTemplate>
    </emulators:GameAspectWrapper.Template>
  </emulators:GameAspectWrapper>

</Grid>