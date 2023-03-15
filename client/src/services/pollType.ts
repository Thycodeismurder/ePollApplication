export type Poll = {
  Id: number;
  Title: string;
  PollId: string;
  Options: [
    {
      id: number;
      title: string;
      votes: number;
    }
  ];
};
