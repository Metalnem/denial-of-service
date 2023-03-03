#!/bin/sh

sudo apt-get update
sudo apt-get install -y build-essential libssl-dev git zlib1g-dev
sudo git clone https://github.com/giltene/wrk2.git
cd wrk2
sudo make
sudo cp wrk /usr/local/bin
