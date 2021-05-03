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
    <RowDefinition Height="*"/>
  </Grid.RowDefinitions>
  <Grid.ColumnDefinitions>
    <ColumnDefinition Width="*"/>
    <ColumnDefinition Width="*"/>
  </Grid.ColumnDefinitions>

  <Label x:Name="MediaItemDescription" Wrap="True" TextTrimming="WordEllipsis" Opacity="0.6" Height="260" HorizontalContentAlignment="Right"
         HorizontalAlignment="Stretch" FontSize="32" Color="{ThemeResource TextColor}" Margin="60,4,40,2"
         Content="{Binding Description}" IsVisible="{Binding Path=[Emulators.Description],Converter={StaticResource ReferenceToBoolConverter}}"/>
  
  <emulators:GameAspectWrapper MediaItem="{Binding Source={StaticResource FanArtBackgroundModel},Path=MediaItem}" Grid.Column="1" >
    <emulators:GameAspectWrapper.Template>
      <ControlTemplate>
        <StackPanel Margin="10,0,10,0" VerticalAlignment="Top" Orientation="Vertical"
                    DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}">

          <Label x:Name="GameName" DataContext="{Binding RelativeSource={RelativeSource TemplatedParent}}"
                 Content="{Binding GameName}" FontSize="{ThemeResource NormalFontSize}" Wrap="True"
                 VerticalAlignment="Stretch" FontFamily="SegoeUIBold"
                 Color="{ThemeResource TextColor}"/>

          <StackPanel Orientation="Horizontal" HorizontalAlignment="Left" Margin="0,0,0,5" VerticalAlignment="Center">
            <StarRatingPanel x:Name="RatingReadOnly" IsReadOnly="True"
                             StarReadOnlyTemplate="{StaticResource StarReadOnlyTemplate}" Maximum="5"
                             IsVisible="{Binding Path=Rating,Converter={StaticResource ReferenceToBoolConverter}}">
              <StarRatingPanel.Value>
                <MultiBinding Converter="{StaticResource ExpressionMultiValueConverter}" ConverterParameter="{}{0}/2">
                  <Binding Path="Rating"/>
                </MultiBinding>
              </StarRatingPanel.Value>
              <StarRatingPanel.LayoutTransform>
                <TransformGroup>
                  <ScaleTransform ScaleX="1.6" ScaleY="1.5"/>
                </TransformGroup>
              </StarRatingPanel.LayoutTransform>
            </StarRatingPanel>
            <Label x:Name="RatingCountLabel" Color="{ThemeResource TextColorForeground}" VerticalAlignment="Center"
                   IsVisible="{Binding Path=Rating,Converter={StaticResource ReferenceToBoolConverter}}"
                   FontSize="{ThemeResource SmallerFontSize}" Margin="15,2,5,1">
              <Label.Content>
                <Binding Path="Rating" Converter="{StaticResource StringFormatConverter}" ConverterParameter="{}({0:F1})"/>
              </Label.Content>
            </Label>
          </StackPanel>

          <Grid IsVisible="{Binding ElementName=Year, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
              <Image Source="HomeContentIcons\year.png" Height="27" Width="27" Stretch="Uniform" Margin="0,-1,5,1" 
                     VerticalAlignment="Center" HorizontalAlignment="Left"/>
              <Label x:Name="Year" HorizontalAlignment="Left" FontSize="{ThemeResource SmallFontSize}" 
                     Color="{ThemeResource TextColorForeground}" VerticalAlignment="Center" Margin="40,0,0,0"
                     Content="{Binding Year}" />
          </Grid>

          <Grid IsVisible="{Binding ElementName=Genre, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
            <Image Source="HomeContentIcons\genre.png" Height="30" Width="30" Stretch="Uniform" Margin="0,0,5,0" 
                   VerticalAlignment="Center" HorizontalAlignment="Left"/>
            <Label x:Name="Genre" FontSize="{ThemeResource SmallFontSize}" TextTrimming="WordEllipsis" Margin="40,0,0,0"
                   Content="{Binding Genres}" Color="{ThemeResource TextColorForeground}" VerticalAlignment="Center"/>
          </Grid>

          <Grid IsVisible="{Binding ElementName=Platform, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
            <Image Source="HomeContentIcons\game.png" Height="30" Width="30" Stretch="Uniform" Margin="0,0,5,0" 
                   VerticalAlignment="Center" HorizontalAlignment="Left"/>
            <Label x:Name="Platform" FontSize="{ThemeResource SmallFontSize}" TextTrimming="WordEllipsis" Margin="40,0,0,0"
                   Content="{Binding Platform}" Color="{ThemeResource TextColorForeground}" VerticalAlignment="Center"/>
          </Grid>

          <Grid IsVisible="{Binding ElementName=Developer, Path=Content, Converter={StaticResource EmptyStringToFalseConverter}}">
            <Image Source="HomeContentIcons\brand.png" Height="30" Width="30" Stretch="Uniform" Margin="0,0,5,0" 
                   VerticalAlignment="Center" HorizontalAlignment="Left"/>
            <Label x:Name="Developer" FontSize="{ThemeResource SmallFontSize}" TextTrimming="WordEllipsis" Margin="40,0,0,0"
                   Content="{Binding Developer}" Color="{ThemeResource TextColorForeground}" VerticalAlignment="Center"/>
          </Grid>

        </StackPanel>
      </ControlTemplate>
    </emulators:GameAspectWrapper.Template>
  </emulators:GameAspectWrapper>

</Grid>
