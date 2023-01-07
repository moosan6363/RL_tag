# installation
1. install ml-agents
    ```
        git clone https://github.com/Unity-Technologies/ml-agents.git -b release_20 --depth 1
    ```
1. Set up a python virtual environment
    ```
        conda create -n ml-agents python=3.10.8
        conda activate ml-agents
        pip install -e ./ml-agents-envs 
        pip install -e ./ml-agents
        pip uninstall grpcio
        conda install grpcio
    ```

# Reference
1. <https://github.com/Unity-Technologies/ml-agents/tree/release_19_docs/docs/>
1. <https://note.com/npaka/n/n30707ea92bd1>
1. <https://github.com/adap/flower/discussions/1010>
