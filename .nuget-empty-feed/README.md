# Intentionally empty NuGet source

This directory is the only package source configured by `../NuGet.config`. It contains no
packages on purpose: M1 restores from the already populated, read-only global package cache
(`F:\nuget\.nuget\packages`). If a required package is absent, restore fails with NU1101 rather
than contacting a feed or writing to a shared cache.
