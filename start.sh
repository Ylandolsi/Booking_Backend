#!/bin/bash

set -e

echo "🔍 Defining ports manually (from docker-compose)..."

# Declare ports as an array
ports=(6379 5432 5001 5000 8081)

echo "🔧 Checking and freeing used ports..."
for port in "${ports[@]}"; do
  pid=$(sudo lsof -ti :$port)
    if [ -n "$pid" ]; then
      echo "❌ Port $port is in use by PID $pid."
      read -p "Do you want to kill it? [y/N] " choice
      if [[ "$choice" =~ ^[Yy]$ ]]; then
        sudo kill -9 $pid
        echo "☠️  Killed PID $pid"
      else
        echo "⏭️ Skipped killing PID $pid"
      fi
  else
    echo "✅ Port $port is free."
  fi
done
echo "🚀 Starting docker compose..."
docker compose up -d
