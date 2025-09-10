import type { AttachmentType } from './enums/AttachmentType';

export default interface MessageAttachment {
  id: string;
  attachmentId: string;
  fileName: string;
  attachmentType: AttachmentType;
  url: string;
  size: number;
}
