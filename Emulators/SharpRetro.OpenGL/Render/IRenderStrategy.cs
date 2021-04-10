namespace SharpRetro.OpenGL.Render
{
  public interface IRenderStrategy
  {
    void Create();
    void Destroy();
    void Render(int width, int height, bool bottomLeftOrigin, uint texture);
  }
}