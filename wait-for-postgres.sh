#!/bin/sh
until nc -z $1 $2; do
    echo "Waiting for PostgreSQL at $1:$2..."
    sleep 2
done
exec $@