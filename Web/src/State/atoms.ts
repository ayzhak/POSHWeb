import {atom, selector, selectorFamily} from "recoil";
import {PSScript, ScriptApi, Job, JobApi} from "../apiClient";

export const scriptsState = atom<Array<PSScript>>({
    key: 'scripsState',
    default: [],
});

// enum with connection state
export enum ConnectionState {
    CONNECTED,
    DISCONNECTED,
    CONNECTING,
}

export const signalRConnectionState = atom<ConnectionState>({
    key: 'singalRConnectionState',
    default: ConnectionState.DISCONNECTED,
});

export const scriptRunState = atom<PSScript | null>({
    key: 'scriptRunState',
    default: null,
});

export const scriptQuery = selectorFamily({
    key: 'scriptQuery',
    get: id => async () => {
        const scriptsApi = new ScriptApi();
        const response = await scriptsApi.apiV1ScriptIdGet(Number(id))
        return response.data;
    },
});

export const scriptsQuery = selector({
    key: 'scriptQuery',
    get: async () => {
        const scriptsApi = new ScriptApi();
        const response = await scriptsApi.apiV1ScriptGet()
        return response.data;
    },
});

export const scriptsJobsState = atom<Array<Job>>({
    key: 'scriptsJobsState',
    default: [],
});

export const scriptJobsQuery = selector({
    key: 'scriptJobsQuery',
    get: async () => {
        const scriptsApi = new JobApi();
        const response = await scriptsApi.apiV1JobGet()
        return response.data;
    },
});

export const scriptJobQuery = selectorFamily({
    key: 'scriptJobQuery',
    get: id => async () => {
        const scriptsApi = new JobApi();
        const response = await scriptsApi.apiV1JobIdGet(Number(id))
        return response.data;
    },
});

export const scriptJobState = atom<Job | null>({
    key: 'scriptJobState',
    default: null,
});

