﻿namespace CodingChallenge;

public class EnvironmentExiter : IEnvironmentExiter
{
    public void Exit(int exitCode) => Environment.Exit(exitCode);
}