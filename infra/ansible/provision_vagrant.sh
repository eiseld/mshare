ansible-playbook playbook_provision.yml -i inventory --extra-vars "variable_host=local_vagrant_dev_01 variable_user=vagrant" --key-file ssh/id_rsa_mshare
