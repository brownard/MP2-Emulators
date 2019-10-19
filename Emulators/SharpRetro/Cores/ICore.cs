using SharpRetro.LibRetro;
using System;

namespace SharpRetro.Cores
{
  /// <summary>
  /// Implementations of this interface provide a managed wrapper for a library implementing the libretro API.
  /// </summary>
  public interface ICore : IDisposable
  {
    /// <summary>
    /// Sets the environment callback. Must be called before <see cref="Init"/>.
    /// </summary>
    /// <param name="environmentCallback">Delegate to use for the callback.</param>
    void SetEnvironment(Func<RETRO_ENVIRONMENT, IntPtr, bool> environmentCallback);

    /// <summary>
    /// Sets the video refresh callback.
    /// </summary>
    /// <param name="videoRefreshCallback">Delegate to use for the callback.</param>
    void SetVideoRefresh(Action<IntPtr, uint, uint, uint> videoRefreshCallback);

    /// <summary>
    /// Sets the audio sample callback.
    /// </summary>
    /// <param name="audioSampleCallback">Delegate to use for the callback.</param>
    void SetAudioSample(Action<short, short> audioSampleCallback);

    /// <summary>
    /// Sets the audio sample batch callback.
    /// </summary>
    /// <param name="audioSampleBatchCallback">Delegate to use for the callback.</param>
    void SetAudioSampleBatch(Func<IntPtr, uint, uint> audioSampleBatchCallback);

    /// <summary>
    /// Sets the input poll callback.
    /// </summary>
    /// <param name="inputPollCallback">Delegate to use for the callback.</param>
    void SetInputPoll(Action inputPollCallback);

    /// <summary>
    /// Sets the input state callback.
    /// </summary>
    /// <param name="inputStateCallback">Delegate to use for the callback.</param>
    void SetInputStateCallback(Func<uint, uint, uint, uint, short> inputStateCallback);

    /// <summary>
    /// Inititializes the core.
    /// </summary>
    void Init();

    /// <summary>
    /// Deinitializes the core.
    /// </summary>
    void Deinit();

    /// <summary>
    /// Gets the API version used by the core.
    /// </summary>
    /// <returns>The API version.</returns>
    uint ApiVersion();

    /// <summary>
    /// Gets statically known system info. Can be called at any time,
    /// even before <see cref="Init"/>.
    /// </summary>
    /// <param name="info"></param>
    void GetSystemInfo(ref retro_system_info info);

    /// <summary>
    /// Gets information about the system audio/video timings and geometry.
    /// Should only be called after a successful call to <see cref="LoadGame(ref retro_game_info)"/>
    /// or <see cref="LoadGameSpecial(uint, ref retro_game_info, uint)"/>.
    /// </summary>
    /// <param name="avInfo"></param>
    void GetSystemAVInfo(ref retro_system_av_info avInfo);

    /// <summary>
    /// Sets the device to be used for the <paramref name="port"/>.
    /// By default, <see cref="RETRO_DEVICE.JOYPAD"/> is assumed to be
    /// plugged in to all available ports.
    /// </summary>
    /// <remarks>
    /// Setting a particular device type is not a guarantee that the core
    /// will only poll input for that particular type. It is only a
    /// hint to the core when it cannot automatically detect the type on its
    /// own. It is also relevant when a core can change its behaviour depending
    /// on the device type.
    /// </remarks>
    /// <param name="port">The port to set the device on.</param>
    /// <param name="device">The <see cref="RETRO_DEVICE"/> to set.</param>
    void SetControllerPortDevice(uint port, uint device);

    /// <summary>
    /// Resets the current game.
    /// </summary>
    void Reset();

    /// <summary>
    /// Runs the game for one video frame.
    /// </summary>
    /// <remarks>
    /// During a call to <see cref="Run"/>, the core must call the callbacks
    /// specified in <see cref="SetVideoRefresh(Action{IntPtr, uint, uint, uint})"/>
    /// and <see cref="SetInputPoll(Action)"/> at least once.
    /// </remarks>
    void Run();

    /// <summary>
    /// Returns the amount of data the core requires to serialize save states.
    /// </summary>
    /// <remarks>
    /// Between calls to <see cref="LoadGame(ref retro_game_info)"/> and
    /// <see cref="UnloadGame"/> the returned size is never allowed to be greater
    /// than the previous returned value, to ensure that the frontend can allocate
    /// a save state buffer once.
    /// </remarks>
    /// <returns>The maximum size in bytes of the save state.</returns>
    uint SerializeSize();

    /// <summary>
    /// Serializes save states to the specified buffer.
    /// </summary>
    /// <param name="data">Pointer to a buffer to save the state.</param>
    /// <param name="size">The size of the buffer.</param>
    /// <returns>
    /// <c>false</c> if serializing failed or <paramref name="size"/>
    /// is lower than the value returned by <see cref="SerializeSize"/>. 
    /// </returns>
    bool Serialize(IntPtr data, uint size);

    /// <summary>
    /// Unserializes save states from the specified buffer.
    /// </summary>
    /// <param name="data">Pointer to a buffer containing the state.</param>
    /// <param name="size">The size of the buffer.</param>
    /// <returns><c>true</c> if deserializing was successful.</returns>
    bool Unserialize(IntPtr data, uint size);

    /// <summary>
    /// Sets a cheat.
    /// </summary>
    /// <param name="index"></param>
    /// <param name="enabled"></param>
    /// <param name="code"></param>
    void CheatSet(uint index, bool enabled, string code);

    /// <summary>
    /// Resets any cheats applied by <see cref="CheatSet(uint, bool, string)"/>.
    /// </summary>
    void CheatReset();

    /// <summary>
    /// Loads a game.
    /// </summary>
    /// <param name="game">Game info to load.</param>
    /// <returns><c>true</c> if the game was loaded successfully.</returns>
    bool LoadGame(ref retro_game_info game);

    /// <summary>
    /// Loads a "special" kind of game. Should not be used,
    /// except in extreme cases.
    /// </summary>
    /// <param name="gameType">Type of game to load.</param>
    /// <param name="game">Game to load.</param>
    /// <param name="numInfo"></param>
    /// <returns></returns>
    bool LoadGameSpecial(uint gameType, ref retro_game_info game, uint numInfo);

    /// <summary>
    /// Unloads a currently loaded game.
    /// </summary>
    void UnloadGame();

    /// <summary>
    /// Gets the region of the game.
    /// </summary>
    /// <returns>The region.</returns>
    uint GetRegion();

    /// <summary>
    /// Gets a region of memory.
    /// </summary>
    /// <param name="id">The type of memory to get.</param>
    /// <returns>Pointer to a buffer containing the memory region.</returns>
    IntPtr GetMemoryData(RETRO_MEMORY id);

    /// <summary>
    /// Gets the size of a region of memory.
    /// </summary>
    /// <param name="id">The type of memory to get the size of.</param>
    /// <returns>The size of the memory region.</returns>
    uint GetMemorySize(RETRO_MEMORY id);

  }
}