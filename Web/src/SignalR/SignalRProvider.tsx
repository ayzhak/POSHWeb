import React, {useEffect, useState} from "react";
import {HubConnection, HubConnectionBuilder} from "@microsoft/signalr";
import {getRecoil, setRecoil} from "recoil-nexus";
import {ConnectionState, scriptJobState, scriptsJobsState, scriptsState, signalRConnectionState} from "../State/atoms";
import {toast} from "react-toastify";
import {useSetRecoilState} from "recoil";
import {JobApi, JobState, PSScript} from "../apiClient";


export default function SignalRProvider() {
    const [connection, setConnection] = useState<null | HubConnection>(null);
    const setConnectionState = useSetRecoilState(signalRConnectionState);
    useEffect(() => {
        const connect = new HubConnectionBuilder()
            .withUrl("http://localhost:5223/signalr/hubs")
            .withAutomaticReconnect()
            .build();

        setConnection(connect);
    }, []);

    useEffect(() => {
            if (connection) {
                connection.start().then(() => {
                        connection.on("ReceiveScriptChanged", (script: PSScript) => {
                            const tmpscripts = getRecoil(scriptsState);
                            let scripts = [...tmpscripts];
                            const index = scripts.findIndex((s: PSScript) => s.id === script.id);
                            if (index === -1) return;
                            scripts[index] = script;
                            setRecoil(scriptsState, scripts);
                        });

                        connection.on("ReceiveScriptCreated", (script: PSScript) => {
                            const tmpscripts = getRecoil(scriptsState);
                            let scripts = [...tmpscripts];
                            scripts.push(script);
                            setRecoil(scriptsState, scripts);
                        });

                        // Remove script when ReceivedScriptRemove event is received
                        connection.on("ReceiveScriptRemoved", (id: number) => {
                            const tmpscripts = getRecoil(scriptsState);
                            let scripts = [...tmpscripts];
                            const index = scripts.findIndex((s: PSScript) => s.id === id);
                            if (index === -1) return;
                            scripts.splice(index, 1);
                            setRecoil(scriptsState, scripts);
                        });
                        connection.on("ReceiveJobUpdate", async (id: number) => {
                            let jobsApi = new JobApi()
                            let job = await jobsApi.apiV1JobIdGet(id);
                            setRecoil(scriptJobState, job.data);
                        });

                        connection.on("ReceiveJobStateChanged", (id: number, state: JobState) => {
                            const tmpjobs = getRecoil(scriptsJobsState);
                            let jobs = [...tmpjobs];
                            const index = jobs.findIndex((s: PSScript) => s.id === id);
                            if (index !== -1) {
                                jobs[index].state = state;
                                setRecoil(scriptsJobsState, jobs);
                            }
                            const tmpjob = getRecoil(scriptJobState);
                            let job = {...tmpjob}
                            if (job && job.id === id) {
                                job.state = state;
                                setRecoil(scriptJobState, job);
                            }
                        });
                        connection.onclose(error => {
                            if (error) {
                                toast.error(error.message);
                            }
                            setConnectionState(ConnectionState.DISCONNECTED)
                        });
                        connection.onreconnecting(error => {
                            if (error) {
                                toast.error(error.message);
                            }
                            setConnectionState(ConnectionState.CONNECTING)
                        });
                        connection.onreconnected(connectionId => {
                            setConnectionState(ConnectionState.CONNECTED)
                        });
                        setConnectionState(ConnectionState.CONNECTED)
                    }
                ).catch(err => {
                    toast.error("Failed to connect to SignalR");
                    setConnectionState(ConnectionState.DISCONNECTED)
                });
            }
        }
    )
    return null;
}
