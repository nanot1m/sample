#!/bin/bash
dotnet ImageFilter/bin/Debug/netcoreapp2.0/Vostok.Sample.ImageFilter.dll &
dotnet ImageStore/bin/Debug/netcoreapp2.0/Vostok.Sample.ImageStore.dll &
dotnet VotingService/bin/Debug/netcoreapp2.0/Vostok.Sample.VotingService.dll
