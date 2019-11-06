# Usage
Copy your public key as ./id_rsa.pub 

## Start vm with vagrant:
vagrant up
## Provision to vagrant:
vagrant provisison
## Provision to remote:

## SSH to vagrant
vagrant ssh
or
ssh vagrant@192.168.50.4 

## Ping a machine with ansible
ansible -m ping -i inventory local_vagrant_dev_01