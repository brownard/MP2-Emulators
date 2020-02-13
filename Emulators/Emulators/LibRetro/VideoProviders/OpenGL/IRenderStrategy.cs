namespace Emulators.LibRetro.VideoProviders.OpenGL
{
  public interface IRenderStrategy
  {
    void Init();
    void Destroy();
    void Render(int width, int height, bool bottomLeftOrigin, uint texture);
  }
}