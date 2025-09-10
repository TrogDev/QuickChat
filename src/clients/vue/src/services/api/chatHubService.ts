import type ChatParticipant from '@/models/ChatParticipant';
import type Message from '@/models/Message';
import type SystemMessage from '@/models/SystemMessage';
import * as signalR from '@microsoft/signalr';

type Handlers = {
  MessageAdded: (message: Message) => void;
  MessageEdited: (message: Message) => void;
  MessageDeleted: (message: Message) => void;
  SystemMessageAdded: (message: SystemMessage) => void;
  UserJoined: (chatId: string, chatParticipant: ChatParticipant) => void;
};

class ChatHubService {
  private handlers: {
    [K in keyof Handlers]: Record<number, Handlers[K]>;
  } = {
    MessageAdded: {},
    MessageEdited: {},
    MessageDeleted: {},
    SystemMessageAdded: {},
    UserJoined: {},
  };

  private connection: signalR.HubConnection | null = null;
  private counter: number = 0;
  private subscriptions: Set<string> = new Set<string>();

  public async subscribe(chatId: string) {
    if (!this.connection) {
      await this.startConnection();
    }

    if (this.subscriptions.has(chatId)) {
      return;
    }

    this.subscriptions.add(chatId);
    await this.connection!.invoke('SubscribeChat', chatId);
  }

  public async unsubscribe(chatId: string) {
    if (!this.subscriptions.delete(chatId)) {
      return;
    }

    await this.connection!.invoke('UnsubscribeChat', chatId);
  }

  private async startConnection() {
    this.connection = new signalR.HubConnectionBuilder()
      .withUrl(import.meta.env.VITE_API_BASE_URL + '/ws/chats')
      .configureLogging(signalR.LogLevel.Information)
      .build();

    this.registerConnectionHandlers(this.connection);

    await this.connection.start();
  }

  private registerConnectionHandlers(connection: signalR.HubConnection) {
    connection.on('MessageAdded', (message: Message) => {
      for (let id in this.handlers.MessageAdded) {
        this.handlers.MessageAdded[id](message);
      }
    });
    connection.on('MessageEdited', (message: Message) => {
      for (let id in this.handlers.MessageEdited) {
        this.handlers.MessageEdited[id](message);
      }
    });
    connection.on('MessageDeleted', (message: Message) => {
      for (let id in this.handlers.MessageDeleted) {
        this.handlers.MessageDeleted[id](message);
      }
    });
    connection.on('SystemMessageAdded', (message: SystemMessage) => {
      for (let id in this.handlers.SystemMessageAdded) {
        this.handlers.SystemMessageAdded[id](message);
      }
    });
    connection.on('UserJoined', (chatId: string, chatParticipant: ChatParticipant) => {
      for (let id in this.handlers.UserJoined) {
        this.handlers.UserJoined[id](chatId, chatParticipant);
      }
    });
  }

  public addHandler<K extends keyof Handlers>(event: K, handler: Handlers[K]): number {
    const id = ++this.counter;
    (this.handlers[event] as Record<string, Handlers[K]>)[id] = handler;
    return id;
  }

  public removeHandler<K extends keyof Handlers>(event: K, id: number): void {
    delete this.handlers[event][id];
  }
}

export const chatHubService = new ChatHubService();
